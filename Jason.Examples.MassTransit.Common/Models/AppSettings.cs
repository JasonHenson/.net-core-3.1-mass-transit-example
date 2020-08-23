using System;

namespace Jason.Examples.MassTransit.Common.Models
{
    /// <summary>
    /// appsettings.json values are serialized into this object
    /// </summary>
    public class AppSettings
    {
        public RabbitMqSettings RabbitMq { get; set; }
    }

    public sealed class RabbitMqSettings
    {
        public string HostName { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}
