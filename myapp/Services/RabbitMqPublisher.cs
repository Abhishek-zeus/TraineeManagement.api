using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using TraineeManagement.myapp.DTOs;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Utility;

namespace TraineeManagement.myapp.Services
{
    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private readonly RabbitMqSettings _settings;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqSettings> options, ILogger<RabbitMqPublisher> logger)
        {
            _settings = options.Value;
            _logger = logger;

            // s1: Establish Connection Factory settings - Fill form
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            //s2: Open connections to the broker - create a pipeline and inside that a channel
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            //s3: Declare our queue as DURABLE so it survives restarts 
            //step3a : the dead letter exchange
            _channel.ExchangeDeclare(
                exchange: _settings.DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false
            );

            //step3b: the failed queue
            _channel.QueueDeclare(
                queue: _settings.FailedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.QueueBind(
                queue: _settings.FailedQueue,
                exchange: _settings.DeadLetterExchange,
                routingKey: _settings.QueueName
            );

            //step3c: the main queue
            var mainQueueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", _settings.DeadLetterExchange}
            };

            _channel.QueueDeclare(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: mainQueueArgs
            );



        }


        public void PublishSubmissionTask(SubmissionProcessingRequest message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                //Make the message persistent on disk
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                //Track the execution chain using headers
                properties.Headers = new Dictionary<string, object>
                {
                  {"CorrelationId", message.CorrelationId}
                };



                _channel.BasicPublish(
                    exchange: string.Empty, //Default direct routing exchange
                    routingKey: _settings.QueueName,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation("Successfully published to RabbitMQ. MessageId: {MessageId}, CorrelationId: {CorrelationId}", message.MessageId, message.CorrelationId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to broker. System will report downstream failure.");
                throw;
            }
        }


        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }


        public bool IsConnected() =>
        _connection?.IsOpen ?? false;
    }
}