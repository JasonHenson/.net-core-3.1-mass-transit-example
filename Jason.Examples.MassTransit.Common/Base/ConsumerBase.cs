using System;
using Jason.Examples.MassTransit.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jason.Examples.MassTransit.Common.Base
{
    public abstract class ConsumerBase
    {
        protected readonly ILogger Logger;
        protected readonly AppSettings AppSettings;


        protected ConsumerBase(IOptions<AppSettings> appSettings, ILogger logger)
        {
            Logger = logger;
            AppSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            Logger = logger;
        }
    }
}