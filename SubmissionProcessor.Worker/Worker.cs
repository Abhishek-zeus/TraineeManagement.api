using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SubmissionProcessor.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    private const string QueueName = "submission-processing";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _logger.LogInformation("Inside worker");
        InitializeRabbitMq();
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            DispatchConsumersAsync = true 
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
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
            var body = ea.Body.ToArray();
            var jsonMessage = Encoding.UTF8.GetString(body);
            _logger.LogInformation("----MESSAGE RECEIVED FROM QUEUE----");
            _logger.LogInformation("Payload: {payload}", jsonMessage);

            //simulating Day 4 task of phase 3
            await Task.Delay(2000, stoppingToken);

            //send ack receipt safely back to rabbit
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            _logger.LogInformation("Task acknowledged successfully.");
        };

        //Send the next task from the queue to my listener, i am ready
        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }


    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

