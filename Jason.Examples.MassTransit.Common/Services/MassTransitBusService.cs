using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jason.Examples.MassTransit.Common.Services
{
    /// <summary>
    /// Service used to stop and start our Mass Transit Bus
    /// </summary>
    public class MassTransitBusService : IHostedService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<MassTransitBusService> _logger;

        public MassTransitBusService(IBusControl busControl, ILogger<MassTransitBusService> logger)
        {
            _busControl = busControl;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("[MassTransitBusService] Starting Mass Transit bus...");
                var busStarted = false;
                //Keep trying to connect to RabbitMQ until we get a connection
                while (!busStarted)
                    try
                    {
                        //Cancel trying to connect to RabbitMQ after 1 minute.
                        var cts = new CancellationTokenSource(new TimeSpan(0, 1, 0)).Token;
                        var busHandle = await _busControl.StartAsync(cts);
                        await busHandle.Ready;
                        busStarted = true;
                        _logger.LogInformation("[MassTransitBusService] start completed.");
                    }
                    catch (RabbitMqConnectionException ex)
                    {
                        _logger.LogError(ex, "[MassTransitBusService] connecting to RabbitMQ failed.");
                        _logger.LogInformation("[MassTransitBusService] trying to connect to rabbit mq again in 10 seconds.");
                        Thread.Sleep(TimeSpan.FromSeconds(10));
                    }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MassTransitBusService] starting bus failed.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[BusService] Stopping Mass Transit bus...");
            try
            {
                await _busControl.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BusService] stopping bus failed.");
            }
        }
    }
}