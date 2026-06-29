using Microsoft.Extensions.Diagnostics.HealthChecks;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Models;

namespace TraineeManagement.myapp.HealthChecks
{
    public class RabbitMqHealthCheck : IHealthCheck
    {
        private readonly IMessagePublisher _publisher;

        public RabbitMqHealthCheck(IMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var result = _publisher.IsConnected() ? HealthCheckResult.Healthy("RabbitMQ connection open.") : HealthCheckResult.Unhealthy("RabbitMQ connection is closed");
            return Task.FromResult(result);

        }
    }
}