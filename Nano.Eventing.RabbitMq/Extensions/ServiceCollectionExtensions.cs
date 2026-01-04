using System;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Common.Serialization.Json;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;

namespace Nano.Eventing.RabbitMq.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add EasyNetQ Eventiong
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options"></param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEasyNetQEventing(this IServiceCollection services, EventingOptions options)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        var serializerSettings = SerializerSettings.GetDefault();

        services
            .AddEasyNetQ(x =>
            {
                x.Hosts = 
                [
                    new(options.Host, options.Port)
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
            .AddScoped<IEventing, EasyNetQEventing>();

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