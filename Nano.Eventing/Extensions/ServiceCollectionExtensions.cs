using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config.Extensions;
using Nano.Eventing.Handlers;
using Nano.Eventing.Interfaces;
using Nano.Eventing.Providers.EasyNetQ;
using Nano.Models.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Helpers;

namespace Nano.Eventing.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds eventing provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddEventing<TProvider>(this IServiceCollection services)
        where TProvider : class, IEventingProvider, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = services
            .BuildServiceProvider()
            .GetService<EventingOptions>();

        var eventingProvider = new TProvider();

        eventingProvider
            .Configure(services, options);

        services
            .AddEventingHandlers()
            .AddEventingHandlerAttributes()
            .AddEventingHealthChecks<TProvider>(options);

        return services;
    }

    /// <summary>
    /// Adds <see cref="EventingOptions"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddEventing(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        return services
            .AddScoped<IEventing, NullEventing>()
            .AddConfigOptions<EventingOptions>(configuration, EventingOptions.SectionName, out _);
    }

    private static IServiceCollection AddEventingHandlers(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        TypesHelper.GetAllTypes()
            .SelectMany(x => x.GetInterfaces(), (x, y) => new
            {
                Type = x,
                GenericType = y
            })
            .Where(x =>
                !x.Type.IsAbstract &&
                x.Type.IsTypeOf(typeof(IEventingHandler<>)) &&
                x.Type != typeof(EntityEventHandler))
            .GroupBy(x => new
            {
                TypeName = x.Type.FullName,
                GenericTypeName = x.GenericType.FullName
            })
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null)
            .ToList()
            .ForEach(x =>
            {
                var handlerType = x.Type;
                var genericHandlerType = x.GenericType;

                services
                    .AddScoped(genericHandlerType, handlerType);
            });

        return services;
    }
    private static IServiceCollection AddEventingHandlerAttributes(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var handlerType = typeof(EntityEventHandler);
        var genericHandlerType = typeof(IEventingHandler<EntityEvent>);

        services
            .AddScoped(genericHandlerType, handlerType);

        return services;
    }
    private static IServiceCollection AddEventingHealthChecks<TProvider>(this IServiceCollection services, EventingOptions options)
        where TProvider : class, IEventingProvider
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (!options.UseHealthCheck)
            return services;

        if (typeof(TProvider) == typeof(EasyNetQProvider))
        {
            var connectionString = string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password)
                ? $"amqp://{options.Host}:{options.Port}{options.VHost}"
                : $"amqp://{options.Username}:{options.Password}@{options.Host}:{options.Port}{options.VHost}";

            services
                .AddHealthChecks()
                .AddRabbitMQ(rabbitConnectionString: connectionString, failureStatus: options.UnhealthyStatus);
        }

        return services;
    }
}