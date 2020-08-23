using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jason.Examples.MassTransit.Common.Extensions
{
    public static class HostBuilderExtensions
    {
        /// <summary>
        ///     extension method  to build and run host while using ILogger to log out to file.
        /// </summary>
        public static async Task BuildAndRun<T>(this IHostBuilder hostBuilder)
        {
            var host = hostBuilder.Build();

            using var scope = host.Services.CreateScope();
            {
                var services = scope.ServiceProvider;
                var hostEnvironment = services.GetService<IHostEnvironment>();
                var logger = services.GetService<ILogger<T>>();
                logger.LogInformation($"[{nameof(T)}] {hostEnvironment.EnvironmentName}");

                try
                {
                    await host.RunAsync();
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("operation cancelled. Exiting consumer application.");
                }
                finally
                {
                    logger.LogInformation("All threads shut down. Exiting consumer application.");
                }
            }
        }
    }
}