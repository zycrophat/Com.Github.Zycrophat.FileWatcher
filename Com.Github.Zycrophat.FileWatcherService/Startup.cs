using System;
using System.Threading.Tasks;
using Autofac;
using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services;
using HealthChecks.UI.Client;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.ApplicationInsights.WindowsServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Com.Github.Zycrophat.FileWatcherService
{
    public class Startup
    {

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; private set; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // ConfigureServices is where you register dependencies. This gets
        // called by the runtime before the ConfigureContainer method, below.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the collection. Don't build or return
            // any IServiceProvider or the ConfigureContainer method
            // won't get called.
            services.AddOptions();
            services.AddMvc().AddControllersAsServices();

            services
                .AddHealthChecks()
                    .AddDiskStorageHealthCheck(setup =>
                    {
                        setup.AddDrive(@"C:\");
                        setup.AddDrive(@"D:\");
                    })
                    .AddPingHealthCheck(setup =>
                    {
                        setup.AddHost("www.google.com", 10000);
                    })
                    .AddCheck<FileWatcherBackgroundServiceHealthCheck>("FileWatcherBackgroundServiceHealthCheck")
                    .AddTcpHealthCheck(setup =>
                    {
                        setup.AddHost("www.google.com", 443);
                    })
                    .AddApplicationInsightsPublisher();

            services.ConfigureTelemetryModule<DiagnosticsTelemetryModule>((module, o) =>
            {
                module.HeartbeatInterval = TimeSpan.FromSeconds(10);
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<FileWatcherBackgroundService>().SingleInstance().As<IHostedService>().AsSelf();
            builder.RegisterType<NameProvider>();
            builder.RegisterType<TimedHostedService>().As<IHostedService>();
            builder.RegisterType<FileWatcherBackgroundServiceHealthCheck>().SingleInstance().AsSelf();
            // Register your own things directly with Autofac, like:
            // builder.RegisterModule(new MyApplicationModule());
        }
        
        // Configure is where you add middleware. This is called after
        // ConfigureContainer. You can use IApplicationBuilder.ApplicationServices
        // here if you need to resolve things from the container.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // If, for some reason, you need a reference to the built container, you
            // can use the convenience extension method GetAutofacRoot.
            //AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
                })
                .UseHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
        }

    }

}
