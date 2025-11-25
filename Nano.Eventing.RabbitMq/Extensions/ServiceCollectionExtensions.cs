using System;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
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

        // BUG: Look into DI for easynetq, to solve the IOptionsMonitor<EventingOptions>.
        // maybe there is some nuget packages, also we already installed one for Microsoft DI, check it out

        services
            .RegisterEasyNetQ(x =>
            {
                // BUG: This should be IOptionsMonitor<EventingOptions>
                var eventingOptions = x
                    .Resolve<EventingOptions>();
                    
                return new ConnectionConfiguration
                {
                    Hosts =
                    {
                        new HostConfiguration
                        {
                            Host = eventingOptions.Host,
                            Port = eventingOptions.Port
                        }
                    },
                    VirtualHost = eventingOptions.VHost,
                    UserName = eventingOptions.Username,
                    Password = eventingOptions.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(eventingOptions.Heartbeat),
                    Timeout = eventingOptions.Timeout,
                    PrefetchCount = eventingOptions.PrefetchCount
                };
            }, x => x
                .Register(options)
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