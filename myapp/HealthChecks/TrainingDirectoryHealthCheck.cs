using Microsoft.Extensions.Diagnostics.HealthChecks;
using TraineeManagement.myapp.Utility;

namespace TraineeManagement.myapp.HealthChecks
{
    public class TrainingDirectoryHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TrainingDirectoryHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var baseUrl = _configuration["TrainingDirectory:BaseUrl"]??"http://localhost:5050";
                var response = await client.GetAsync($"{baseUrl}/health/live", cancellationToken);

                return response.IsSuccessStatusCode ? HealthCheckResult.Healthy("TrainingDirectory.Api reachable.") : HealthCheckResult.Degraded($"TrainingDirectory.Api returned {response.StatusCode}.");
            }
            catch(Exception ex)
            {
                return HealthCheckResult.Unhealthy("Cannot reach TrainingDirectory.Api.", ex);
            }
        }
    }
}