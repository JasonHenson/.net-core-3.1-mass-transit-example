using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Jason.Examples.MassTransit.Common.Base;
using Jason.Examples.MassTransit.Common.Enums;
using Jason.Examples.MassTransit.Common.Events;
using Jason.Examples.MassTransit.Common.Extensions;
using Jason.Examples.MassTransit.Common.Models;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jason.Examples.MassTransit.Producer
{
    //todo add priority
    //todo comment code
    //todo populate all with settings
    //todo create read me
    //todo clean up logging
    //todo program.cs refactor build and starting the host to be reusable
    public class ProducerBackgroundService : BackgroundServiceBase
    {
        private readonly IBusControl _busControl;

        public ProducerBackgroundService(
            IBusControl busControl,
            IOptions<AppSettings> appSettings,
            ILogger<ProducerBackgroundService> logger) : base(appSettings, logger)
        {
            _busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Starting up producer service...");

            var rnd = new Random();

            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    stoppingToken.ThrowIfCancellationRequested();

                    var myEvent = new MyEvent
                    {
                        Id = RandomNumberGenerator.GetInt32(0, int.MaxValue),
                        EventType = rnd.GetRandomEnum<EventTypeEnum>(),
                        Message = Guid.NewGuid().ToString() //just some random data for testing
                    };

                    var randomPriority = rnd.Next(1, 2);
                    await _busControl.Publish(myEvent,x=>x.SetPriority((byte)2), stoppingToken);

                    Logger.LogInformation($"[Producer]  message published successfully {myEvent}.");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                catch (RabbitMqConnectionException ex)
                {
                    Logger.LogError(ex, "[Producer] failed to connect to rabbit mq.");
                    Logger.LogInformation("[Producer] trying to connect to rabbit mq again in 1 minute.");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
                catch (OperationCanceledException)
                {
                    Logger.LogInformation("[Producer] cancelling operation...");

                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "[Producer] unknown error has occurred.");
                    Logger.LogInformation("[Producer] restarting service in 1 minute.");
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (_busControl != null)
                await _busControl.StopAsync(stoppingToken);

            Logger.LogInformation("[Producer] is stopping.");
        }
    }
}