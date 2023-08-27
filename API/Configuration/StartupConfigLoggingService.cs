
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using NLog;

namespace InventoryManagement.API.Configuration
{
    public class StartupConfigLoggingService
    {
        private IConfiguration _configuration { get; }
        private readonly ILogger _logger;

        public StartupConfigLoggingService(ILogger logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        public void ExecuteStartupLogging()
        {
            var dbConnProps = _configuration["connectionStrings:InventoryManagementAPI_ConnectionString"].Split(";".ToCharArray());
            var properties = new Dictionary<string, string>
                {
                    {"Auth0:ApiIdentifier", _configuration["Auth0:ApiIdentifier"]}, 
                    {"Auth0:Domain", _configuration["Auth0:Domain"]},
                    {"Server", dbConnProps[0].Substring(dbConnProps[0].IndexOf("="), dbConnProps[0].Length - dbConnProps[0].IndexOf("=") - 1)},
                    {"TargetDB", dbConnProps[1].Substring(dbConnProps[1].IndexOf(":"), dbConnProps[1].Length - dbConnProps[1].IndexOf(":") - 1)}
                };

            _logger.Info("AppStartup", properties);
        }
    }
}
