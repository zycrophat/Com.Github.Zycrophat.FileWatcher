using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> logger;
        private Timer _timer;
        private string foo;

        public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration configuration)
        {
            this.logger = logger;

            var fooOption = configuration.GetSection("bla:foo").Get<ProtectedValue>();
            //configuration.GetSection("bla:foo").Bind(fooOption);

            foo = fooOption.TryUnprotect().IfNoneOrFail(fooOption.CipherText);
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            logger.LogInformation(
                "Timed Hosted Service is working. Foo: {Foo}, Count: {Count}", foo, count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
