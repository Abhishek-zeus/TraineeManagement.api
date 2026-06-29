using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.SubmissionProcessor.Worker.Data;
using TraineeManagement.SubmissionProcessor.Worker.Enums;
using TraineeManagement.SubmissionProcessor.Worker.Interfaces;
using TraineeManagement.SubmissionProcessor.Worker.Models;
using TraineeManagement.SubmissionProcessor.Worker.Utility;

namespace TraineeManagement.SubmissionProcessor.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    //Singleton = Created once for entire life of appl, Scoped = A new instance created for each task
    //Used for connecting singleton services wth scoped services
    private IConnection _connection = null!;
    private readonly int _maxAttempts;
    private IModel _channel = null!;
    // private const string queueName = "submission-processing";
    // private const string DeadLetterExchange = "submission-processing.dlx";
    // private const string FailedqueueName = "submission-processing-failed";

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, IOptions<ProcessingSettings> processingOptions)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = serviceScopeFactory;
        _maxAttempts = processingOptions.Value.MaxAttempts;
        _logger.LogInformation("Inside worker");
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
        var queueName = _configuration["RabbitMQ:QueueName"] ?? "submission-processing";
        var dlxExchange = _configuration["RabbitMQ:DeadLetterExchange"] ?? "submission-processing.dlx";
        var failedQueue = _configuration["RabbitMQ:FailedQueueName"] ?? "submission-processing-failed";



        var factory = new ConnectionFactory
        {
            HostName = hostName,
            DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        //step3a: DeadLetterExchange
        _channel.ExchangeDeclare(
                exchange: dlxExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

        //step3b: the failed queue
        _channel.QueueDeclare(
            queue: failedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: failedQueue,
            exchange: dlxExchange,
            routingKey: queueName
        );

        //step3c: the main queue
        var mainQueueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", dlxExchange}
            };

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArgs
        );

        //Fetch exactly 1 message at a time to distribute weight evenly
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

    }



    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>    //ea-EventArguments from Rabbit
        {
            try
            {
                var body = ea.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                _logger.LogInformation("----MESSAGE RECEIVED FROM QUEUE----");
                _logger.LogInformation("Payload: {payload}", jsonMessage);

                /////////simulating Day 4 task of phase 3
                await Task.Delay(2000, stoppingToken);
                SubmissionProcessingRequestedMessage? message;
                try
                {
                    message = JsonSerializer.Deserialize<SubmissionProcessingRequestedMessage>(jsonMessage);
                }
                catch (JsonException ex)    //if the user sends some corrupted data, missing fields instead of json, serializer will crash
                {
                    _logger.LogError(ex, "Could not deserialize message - sending straight to dead-letter");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    //requeue: false will tell rabbitmq, remove from main queue and add to dead queue 
                    return;
                }

                if (message == null)
                {
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                //load scoped services into singleton (Long running worker)
                using var scope = _scopeFactory.CreateScope();
                var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();

                var job = await dbContext.ProcessingJobs.FirstOrDefaultAsync(j =>
                    j.MessageId == message.MessageId, stoppingToken);

                if (job == null)
                {
                    _logger.LogWarning("No ProcessingJob found for MessageId: {MessageId}. Acknowledging and dropping.", message.MessageId);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }


                //-------------IDEMPOTENCY CHECK--------------
                if (job.Status == ProcessingJobStatus.Completed)
                {
                    _logger.LogInformation("Duplicate delivery detected for MessageId: {MessageId}. Already Completed, skipping reprocessing.", message.MessageId);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }
                job.Status = ProcessingJobStatus.Processing;
                job.Attempts += 1;
                job.StartedDate ??= DateTime.UtcNow;
                await dbContext.SaveChangesAsync(stoppingToken);



                //logger Scoped it for a CorrelationId 
                using (_logger.BeginScope("----- CorrelationId : {CorrelationId}", job.CorrelationId))
                {
                    try
                    {
                        var submissionFile = await dbContext.SubmissionFiles.FirstOrDefaultAsync(f =>
                            f.Id == message.FileId, stoppingToken);
                        if (submissionFile == null)
                        {
                            throw new PermanentProcessingException($"SubmissionFile {message.FileId} notfound.");
                        }

                        //deliberate test hooks
                        if (submissionFile.OriginalFileName.Contains("simulate-permanent-failure"))
                        {
                            throw new PermanentProcessingException("Simulated permanent failure (test file name marker).");
                        }
                        if (submissionFile.OriginalFileName.Contains("simulate-transient-failure") && job.Attempts < _maxAttempts)
                        {
                            throw new TransientProcessingException($"Simulated transient failure (attempt {job.Attempts} of {_maxAttempts}).");
                        }

                        //-------REAL SIMULATED PROCESSING OF CHECKSUM----------
                        using var fileStream = await fileStorage.OpenReadAsync(submissionFile.StorageName, stoppingToken);
                        var recomputedHashBytes = await SHA256.HashDataAsync(fileStream, stoppingToken);
                        var recomputedHash = Convert.ToHexString(recomputedHashBytes).ToLowerInvariant();

                        if (recomputedHash != submissionFile.CheckSum)
                        {
                            _logger.LogWarning("Checksum mismatch for FileId: {FileId}. Stored: {Stored}, Recomputed: {Recomputed}", submissionFile.Id, submissionFile.CheckSum, recomputedHash);
                        }


                        //New in Day 5: Look up the trainee via directory service
                        var directoryClient = scope.ServiceProvider.GetRequiredService<ITrainingDirectoryClient>();
                        var submission = await dbContext.Submissions.FirstOrDefaultAsync(s => s.Id == message.SubmissionId, stoppingToken); //I have submission -> taskAssignmentId now 
                        if (submission != null)
                        {
                            var assignment = await dbContext.TaskAssignments.FirstOrDefaultAsync(a => a.Id == submission.TaskAssignmentId, stoppingToken); // now i have the taskAssignment -> traineeId
                            if (assignment != null)
                            {
                                try
                                {
                                    var profile = await directoryClient.GetTraineeProfileAsync(assignment.TraineeId, job.CorrelationId, stoppingToken);

                                    if (profile != null)
                                    {
                                        _logger.LogInformation("Trainee profile retrieved. TraineeId: {TraineeId}, Name: {FullName}, CorrelationId: {CorrelationId}", profile.TraineeId, profile.FullName, job.CorrelationId);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Trainee {TraineeId} not found in directory service. Continuing without profile data.", assignment.TraineeId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //FallBack behaviour --Polly will handle it
                                    _logger.LogWarning(ex, "Directory service unavailable for TraineeId:{TraineeId}. Proceeding without profile data. JobId: {JobId}", assignment.TraineeId, job.Id);
                                }

                            }

                        }


                        job.Status = ProcessingJobStatus.Completed;
                        job.CompletedDate = DateTime.UtcNow;
                        job.ErrorSummary = null;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        _logger.LogInformation("Job Completed. JobId: {JobId}, FileId: {FileId}, Attempts: {Attempts}", job.Id, job.FileId, job.Attempts);
                    }
                    catch (PermanentProcessingException ex)
                    {
                        _logger.LogError("Permanent failure — routing straight to dead-letter without retry. JobId: {JobId}, Error: {Error}", job.Id, ex.Message);
                        job.Status = ProcessingJobStatus.Failed;
                        job.ErrorSummary = ex.Message;
                        job.CompletedDate = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                    catch (Exception ex) //Transient or any unclassified exception : safer to retry
                    {
                        job.ErrorSummary = ex.Message;
                        if (job.Attempts >= _maxAttempts) //If attempts exhausted, go to deadlock with requeue: false
                        {
                            _logger.LogError("Job exhausted retries ({Attempts}/{Max}). Routing to dead-letter. JobId: {JobId}", job.Attempts, _maxAttempts, job.Id);
                            job.Status = ProcessingJobStatus.Failed;
                            job.CompletedDate = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                        }
                        else    //if attempts are left, retry with requeue: true
                        {
                            _logger.LogError("Transient failure on attempt {Attempts}/{Max}. Will retry. JobId: {JobId}, Error: {Error}", job.Attempts, _maxAttempts, job.Id, ex.Message);
                            job.Status = ProcessingJobStatus.Queued;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                        }
                    }
                }

            }
            catch (Exception globalEx)
            {
                // THIS WILL CATCH ANY DATABASE CONNECTION TIMEOUTS OR SYNTAX CRASHES
                _logger.LogCritical(globalEx, "CRITICAL: The consumer handler crashed entirely!");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        //Send the next task from the queue to my listener, i am ready
        _channel.BasicConsume(queue: _configuration["RabbitMQ:QueueName"] ?? "submission-processing", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }


    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }



    public class SubmissionProcessingRequestedMessage
    {
        public string MessageId { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;
        public int SubmissionId { get; set; }
        public int FileId { get; set; }
        public DateTime RequestedAt { get; set; }
        public string ContractVersion { get; set; } = "1.0";
    }
}

