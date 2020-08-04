using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Com.Github.Zycrophat.FileWatcherService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            // ASP.NET Core 3.0+:
            // The UseServiceProviderFactory call attaches the
            // Autofac provider to the generic hosting mechanism.
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseWindowsService()
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                    .UseContentRoot(AppContext.BaseDirectory)
                    
                    .UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();
                    var env = hostingContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                )
                .Build();

            host.Run();
        }



    }
}
