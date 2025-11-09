using System;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Serialization.Json.Const;

namespace Nano.Eventing.Providers.EasyNetQ.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add EasyNetQ Eventiong
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="options">The <see cref="EventingOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEasyNetQEventing(this IServiceCollection services)
    {
        if (services == null) 
            throw new ArgumentNullException(nameof(services));

        services
            .RegisterEasyNetQ(x =>
            {
                var options = x
                    .Resolve<EventingOptions>();
                    
                return new ConnectionConfiguration
                {
                    Hosts =
                    {
                        new HostConfiguration
                        {
                            Host = options.Host,
                            Port = options.Port
                        }
                    },
                    VirtualHost = options.VHost,
                    UserName = options.Username,
                    Password = options.Password,
                    RequestedHeartbeat = TimeSpan.FromSeconds(options.Heartbeat),
                    Timeout = TimeSpan.FromSeconds(options.Timeout),
                    PrefetchCount = options.PrefetchCount
                };
            }, x => x
                .Register<ISerializer>(_ =>
                {
                    var serializerSettings = SerializerSettings.GetDefault();

                    return new NewtonsoftJsonSerializer(serializerSettings);
                }));

        services
            .AddScoped<IEventing, EasyNetQEventing>();

        services
            .AddHealthChecks()
            .AddRabbitMqHealthChecks();

        return services;
    }
}