using System;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.Common.Extensions;
using Nano.Common.Serialization;
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

        services
            .RegisterEasyNetQ(x =>
            {
                var eventingOptions = x
                    .Resolve<IOptionsMonitor<EventingOptions>>();
                    
                return new ConnectionConfiguration
                {
                    Hosts =
                    {
                        new HostConfiguration
                        {
                            Host = eventingOptions.CurrentValue.Host,
                            Port = eventingOptions.CurrentValue.Port
                        }
                    },
                    VirtualHost = eventingOptions.CurrentValue.VHost,
                    UserName = eventingOptions.CurrentValue.Username,
                    Password = eventingOptions.CurrentValue.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(eventingOptions.CurrentValue.Heartbeat),
                    Timeout = eventingOptions.CurrentValue.Timeout,
                    PrefetchCount = eventingOptions.CurrentValue.PrefetchCount
                };
            }, x => x
                .Register<ISerializer>(_ =>
                {
                    var serializerSettings = SerializerSettings.GetDefault();

                    return new NewtonsoftJsonSerializer(serializerSettings);
                }));

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