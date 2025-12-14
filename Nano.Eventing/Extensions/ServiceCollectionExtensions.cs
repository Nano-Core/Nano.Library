using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;
using System;
using System.Linq;

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

        services
            .AddConfigSection<EventingOptions>(EventingOptions.SectionName, out var options);

        if (options == null)
        {
            throw new NullReferenceException(nameof(options));
        }

        var provider = Activator.CreateInstance<TProvider>();
        provider
            .Configure(services, options);

        services
            .AddSingleton<IEventingProvider>(provider)
            .AddEventingHandlers();

        services
            .AddSingleton<IRegisterEventHandlersTask, RegisterEventHandlersTask>();

        return services;
    }


    private static IServiceCollection AddEventingHandlers(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var eventHandlerTypes = TypesHelper
            .GetAllTypes()
            .SelectMany(x => x.GetInterfaces(), (x, y) => new
            {
                Type = x,
                GenericType = y
            })
            .Where(x =>
                !x.Type.IsAbstract &&
                !x.Type.IsGenericType &&
                x.Type.IsTypeOf(typeof(IEventingHandler<>)))
            .GroupBy(x => new
            {
                TypeName = x.Type.FullName,
                GenericTypeName = x.GenericType.FullName
            })
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null);

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            var serviceType = eventHandlerType.GenericType;
            var implementationType = eventHandlerType.Type;

            services
                .AddScoped(serviceType, implementationType);
        }

        return services;
    }
}