using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace InventoryManagement.API.Health
{
    public class AppConfigurationHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        private readonly string[] _appSettings =
        {
             "Auth0:ApiIdentifier", "Auth0:Domain", "Subscription:DefaultName", "connectionStrings:InventoryManagementAPI_ConnectionString"
        };
        public AppConfigurationHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var healthCheckResult = await Task.Run(CheckAppSettingsHealth, cancellationToken);
                return healthCheckResult;
            }
            catch (Exception)
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        private HealthCheckResult CheckAppSettingsHealth()
        {
            var missingAppSettings = _appSettings.Where(appSetting => string.IsNullOrEmpty(_configuration[appSetting]))
                .Where(appSetting => string.IsNullOrEmpty(Environment.GetEnvironmentVariable(appSetting)))
                .Select(appSetting => appSetting).ToList();
            return missingAppSettings.Any()
                ? HealthCheckResult.Unhealthy($"Some App Settings are missing. {missingAppSettings}")
                : HealthCheckResult.Healthy();
        }
    }
}
