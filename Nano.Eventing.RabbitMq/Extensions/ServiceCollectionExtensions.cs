using System;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Common.Serialization.Json;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;

namespace Nano.Eventing.RabbitMq.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register eventing services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers EasyNetQ-based RabbitMQ eventing services in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which eventing services are added.</param>
    /// <param name="options">Configuration options for RabbitMQ eventing, including connection settings, heartbeat, timeout, prefetch count, and health check options.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the eventing services registered.</returns>
    /// <remarks>
    ///     This method registers:
    ///     - EasyNetQ as the RabbitMQ client.
    ///     - <see cref="IEventing"/> as <see cref="RabbitMqEventing"/>.
    ///     - Optional health checks if <see cref="EventingOptions.UseHealthCheck"/> is true.
    ///     It also configures JSON serialization using Newtonsoft.Json with default serializer settings.
    /// </remarks>
    public static IServiceCollection AddEasyNetQEventing(this IServiceCollection services, EventingOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        var serializerSettings = SerializerSettings.GetDefault();

        services
            .AddEasyNetQ(x =>
            {
                x.Hosts =
                [
                    new HostConfiguration(options.Host, options.Port)
                ];
                x.VirtualHost = options.VHost;
                x.UserName = options.Username;
                x.Password = options.Password;
                x.RequestedHeartbeat = TimeSpan.FromSeconds(options.Heartbeat);
                x.Timeout = options.Timeout;
                x.PrefetchCount = options.PrefetchCount;
                x.MandatoryPublish = true;
                x.PublisherConfirms = false;
            })
            .UseNewtonsoftJson(serializerSettings);

        services
            .AddScoped<IEventing, RabbitMqEventing>();

        if (options.UseHealthCheck)
        {
            var failureStatus = options.UnhealthyStatus
                .GetHealthStatus();

            services
                .AddHealthChecks()
                .AddRabbitMqHealthChecks(failureStatus, null, options.Timeout);
        }

        return services;
    }
}