using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<FileWatcherBackgroundServiceHealthCheck> logger;

        public FileWatcherBackgroundServiceHealthCheck(ILogger<FileWatcherBackgroundServiceHealthCheck> logger, FileWatcherBackgroundService fileWatcherBackgroundService)
        {
            this.fileWatcherBackgroundService = fileWatcherBackgroundService;
            this.logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) => 
            await Task.Run(() =>
            {
                logger.LogInformation("checking health");
                bool fileWatcherIsRunning = fileWatcherBackgroundService.IsRunning;
                return fileWatcherIsRunning ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
            });
    }
}
