using System;
using System.Threading.Tasks;
using Autofac;
using AutofacSerilogIntegration;
using Com.Github.Zycrophat.FileWatcherService.Filewatcher.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Com.Github.Zycrophat.FileWatcherService
{
    public class Startup
    {

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;

            Action onChange = async () =>
            {
                Console.WriteLine("Rats are eaten by snakes");
                await Task.Delay(1000);
            };

            //ChangeToken.OnChange(() => configuration.GetReloadToken(), onChange);
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
                    });
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                //.Enrich.WithShortSourceContext()
                //.Enrich.WithThreadId()
                //.MinimumLevel.Debug()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                //.WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fffffff} [{Level:u}] [{ThreadId}] [{SourceContext}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            builder.RegisterLogger();
            builder.RegisterType<FileWatcherBackgroundService>().SingleInstance().As<IHostedService>().AsSelf();
            builder.RegisterType<NameProvider>();
            builder.RegisterType<TimedHostedService>().As<IHostedService>();
            builder.RegisterType<FileWatcherBackgroundServiceHealthCheck>().SingleInstance().As<IHealthCheck>().AsSelf();
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
            //loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //_ = app.UseMvc();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                //endpoints.MapHub<MyChatHub>()
                //endpoints.MapGrpcService<MyCalculatorService>()
            });
            app.UseHealthChecks("/hc", new HealthCheckOptions()
             {
                 Predicate = _ => true,
                 ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
             });
        }

    }

}
