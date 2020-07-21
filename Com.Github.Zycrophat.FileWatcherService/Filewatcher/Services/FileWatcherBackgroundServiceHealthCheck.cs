using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class FileWatcherBackgroundServiceHealthCheck : IHealthCheck
    {
        private readonly FileWatcherBackgroundService fileWatcherBackgroundService;

        private readonly ILogger logger;

        public FileWatcherBackgroundServiceHealthCheck(ILogger logger, FileWatcherBackgroundService fileWatcherBackgroundService)
        {
            this.fileWatcherBackgroundService = fileWatcherBackgroundService;
            this.logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) => 
            await Task.Run(() =>
            {
                logger.Information("checking health");
                bool fileWatcherIsRunning = fileWatcherBackgroundService.IsRunning;
                if (fileWatcherIsRunning)
                {
                    return HealthCheckResult.Healthy();
                }
                else
                {
                    return HealthCheckResult.Unhealthy();
                }
            });
    }
}
