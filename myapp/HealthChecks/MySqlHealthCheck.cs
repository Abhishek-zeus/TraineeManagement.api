using TraineeManagement.myapp.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TraineeManagement.myapp.HealthChecks
{
    public class MySqlHealthCheck : IHealthCheck
    {
        private readonly AppDbContext _context;
        public MySqlHealthCheck(AppDbContext context)
        {
            _context = context;
        }

        //Implementation of IHealthCheck
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken); // returns bool
            return canConnect ? HealthCheckResult.Healthy("MySQL reachable") : HealthCheckResult.Unhealthy("MySQL database is not reachable.");;
        }
    }
}