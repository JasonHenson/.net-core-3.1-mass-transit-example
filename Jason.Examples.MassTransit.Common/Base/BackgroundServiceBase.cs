using System;
using Jason.Examples.MassTransit.Common.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jason.Examples.MassTransit.Common.Base
{
    public abstract class BackgroundServiceBase : BackgroundService
    {
        protected readonly ILogger Logger;
        protected readonly AppSettings AppSettings;


        protected BackgroundServiceBase(IOptions<AppSettings> appSettings, ILogger logger)
        {
            Logger = logger;
            AppSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            Logger = logger;
        }
    }
}