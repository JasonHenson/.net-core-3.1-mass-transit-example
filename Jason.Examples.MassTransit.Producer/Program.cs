using System;
using System.Threading.Tasks;
using Jason.Examples.MassTransit.Common.Extensions;
using Jason.Examples.MassTransit.Common.Models;
using Jason.Examples.MassTransit.Common.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;

namespace Jason.Examples.MassTransit.Producer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<HostOptions>(option => { option.ShutdownTimeout = TimeSpan.FromMinutes(1); });
                    services.Configure<AppSettings>(hostContext.Configuration);
                    services.AddMassTransit(x =>
                    {
                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var appSettings = provider
                                .GetRequiredService<IOptions<AppSettings>>()
                                .Value;

                            cfg.Host(new Uri(appSettings.RabbitMq.HostName), h =>
                            {
                                h.Username(appSettings.RabbitMq.UserName);
                                h.Password(appSettings.RabbitMq.Password);
                            });
                        }));
                    });
                    services.AddHostedService<ProducerBackgroundService>();
                    services.AddSingleton<IHostedService, MassTransitBusService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddDebug();
                    logging.AddNLog(hostingContext.Configuration);
                });

            await hostBuilder.BuildAndRun<Program>();
        }
    }
}