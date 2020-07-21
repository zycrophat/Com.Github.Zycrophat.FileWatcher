﻿using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger logger;
        private Timer _timer;
        private string foo;

        public TimedHostedService(ILogger logger, IConfiguration configuration)
        {
            this.logger = logger;

            var fooOption = configuration.GetSection("bla:foo").Get<ProtectedValue>();
            //configuration.GetSection("bla:foo").Bind(fooOption);

            foo = fooOption.UnprotectedValue;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.Information("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            logger.Information(
                "Timed Hosted Service is working. Foo: {Foo}, Count: {Count}", foo, count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.Information("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
