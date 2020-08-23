using System;
using System.Threading;
using System.Threading.Tasks;
using Jason.Examples.MassTransit.Common.Base;
using Jason.Examples.MassTransit.Common.Enums;
using Jason.Examples.MassTransit.Common.Events;
using Jason.Examples.MassTransit.Common.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jason.Examples.MassTransit.Consumer
{
    public class MyConsumer :
        ConsumerBase,
        IConsumer<MyEvent>
    {
        public MyConsumer(
            IOptions<AppSettings> appSettings,
            ILogger<MyConsumer> logger)
            : base(appSettings, logger)
        {
            Logger.LogInformation(
                $"[{nameof(MyConsumer)}] thread id: {Thread.CurrentThread.ManagedThreadId} consumer created.");
        }

        public async Task Consume(ConsumeContext<MyEvent> context)
        {
            var prefix = $"[{nameof(MyConsumer)}] thread id: {Thread.CurrentThread.ManagedThreadId}";

            Logger.LogInformation($"{prefix} message received: {context.Message}");
            switch (context.Message.EventType)
            {
                case EventTypeEnum.Undefined:
                    throw new ArgumentOutOfRangeException(nameof(context.Message.EventType), $"{prefix} event type is undefined.");
                case EventTypeEnum.Create:
                    Logger.LogInformation($"{prefix} create event handled.");
                    break;
                case EventTypeEnum.Update:
                    Logger.LogInformation($"{prefix} update event handled.");
                    break;
                case EventTypeEnum.Delete:
                    Logger.LogInformation($"{prefix} delete event handled.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(context.Message.EventType), 
                        $"{prefix} unknown event type.");
            }

            await Task.Delay(new Random().Next(1, 10));
            Logger.LogInformation($"{prefix} processing message complete id: {context.Message.Id}.");
        }
    }
}