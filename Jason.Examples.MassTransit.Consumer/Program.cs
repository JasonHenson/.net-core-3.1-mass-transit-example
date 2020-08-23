using System;
using System.Threading.Tasks;
using GreenPipes;
using Jason.Examples.MassTransit.Common.Extensions;
using Jason.Examples.MassTransit.Common.Models;
using Jason.Examples.MassTransit.Common.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;

namespace Jason.Examples.MassTransit.Consumer
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
                        x.AddConsumer<MyConsumer>();
                        x.AddMessageScheduler(new Uri("queue:scheduler"));
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
                            cfg.UseInMemoryScheduler("scheduler-queue");
                            cfg.ReceiveEndpoint(
                                "my-queue",
                                e =>
                                {
                                    e.UseScheduledRedelivery(r =>
                                        r.Intervals(TimeSpan.FromMinutes(1),
                                            TimeSpan.FromMinutes(3),
                                            TimeSpan.FromMinutes(5)));
                                    e.UseMessageRetry(r =>
                                    {
                                        r.Immediate(4);
                                        r.Ignore<ArgumentNullException>();
                                    });

                                    e.PrefetchCount = 32;
                                    e.UseConcurrencyLimit(Environment.ProcessorCount);
                                    e.Consumer<MyConsumer>(provider);
                                });
                        }));
                        services.AddSingleton<IHostedService, MassTransitBusService>();
                    });
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