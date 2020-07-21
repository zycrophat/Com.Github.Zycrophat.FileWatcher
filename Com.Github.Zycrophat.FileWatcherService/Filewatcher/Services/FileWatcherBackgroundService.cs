using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Filesystemwatcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services
{
    public class FileWatcherBackgroundService : BackgroundService
    {
        private readonly ILogger logger;

        private RecoveringFileSystemWatcher FileSystemWatcher;

        private readonly string DirectoryToWatch;

        public FileWatcherBackgroundService(ILogger logger, IConfiguration configuration)
        {
            this.logger = logger;
            DirectoryToWatch = configuration.GetValue<string>("FileWatcherBackgroundService:DirectoryToWatch");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task backgroundServiceStop = Task.Delay(TimeSpan.FromMilliseconds(-1), stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (FileSystemWatcher = CreateFileSystemWatcher(DirectoryToWatch))
                    {
                        logger.Information("starting to watch {directoryToWatch}", DirectoryToWatch);
                        FileSystemWatcher.EnableRaisingEvents = true;

                        try
                        {
                            await backgroundServiceStop;
                        }
                        catch (TaskCanceledException)
                        {
                            logger.Information("background service has been cancelled");
                        }
                        finally
                        {
                            logger.Information("stopping to watch {directoryToWatch}", DirectoryToWatch);
                            FileSystemWatcher.EnableRaisingEvents = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Some error");
                }
                var waitBeforeRetry = Task.Delay(TimeSpan.FromSeconds(5));
                await Task.WhenAny(backgroundServiceStop, waitBeforeRetry);
            }

        }


        private RecoveringFileSystemWatcher CreateFileSystemWatcher(string path)
        {
            var watcher = new RecoveringFileSystemWatcher()
            {
                Path = path,
                IncludeSubdirectories = true
            };
            watcher.Created += ProcessFileSystemEvent;
            watcher.Existed += ProcessFileSystemEvent;
            watcher.Error += OnError;

            return watcher;
        }

        private async void ProcessFileSystemEvent(object source, FileSystemEventArgs e) =>
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
                logger.Information($"File: {e.FullPath} {e.ChangeType}");
            });

        private async void OnError(object source, FileWatcherErrorEventArgs e) => 
            await Task.Run(() =>
            {
                logger.Error($"Error while watching", e.Exception);
            });

        public bool IsRunning {
            get => FileSystemWatcher?.EnableRaisingEvents ?? false;
        }

    }
}
