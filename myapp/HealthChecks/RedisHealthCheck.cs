using TraineeManagement.myapp.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TraineeManagement.myapp.Utility;

namespace TraineeManagement.myapp.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly ICacheService _cache;
        public RedisHealthCheck(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                await _cache.SetAsync("healthcheck:ping", "pong", TimeSpan.FromSeconds(5) ,cancellationToken);
                var result = await _cache.GetAsync<string>("healthcheck:ping", cancellationToken);
                return result == "pong" ? HealthCheckResult.Healthy("Redis reachable.") : HealthCheckResult.Degraded("Redis responded unexpectedly.");
            }
            catch(Exception ex)
            {
                return HealthCheckResult.Unhealthy("Cannot reach Redis.", ex);
            }
        }
    }
}