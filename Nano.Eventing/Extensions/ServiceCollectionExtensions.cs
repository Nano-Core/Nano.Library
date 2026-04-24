using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;
using System;
using System.Linq;
using Nano.Common;

namespace Nano.Eventing.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register Nano eventing services and handlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an eventing provider of type <typeparamref name="TProvider"/> to the service collection,
    /// registers all <see cref="IEventingHandler{TEvent}"/> implementations, and configures the <see cref="EventingOptions"/> section from configuration.
    /// </summary>
    /// <typeparam name="TProvider">The type of the eventing provider. Must implement <see cref="IEventingProvider"/> and have a parameterless constructor.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to which eventing services are added.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the eventing provider and handlers registered.</returns>
    /// <remarks>
    ///     This method performs the following actions:
    ///     <list type="bullet">
    ///     <item>Registers the <see cref="EventingOptions"/> configuration section.</item>
    ///     <item>Creates an instance of <typeparamref name="TProvider"/> and invokes its <see cref="IEventingProvider.Configure"/> method.</item>
    ///     <item>Registers <typeparamref name="TProvider"/> as a singleton <see cref="IEventingProvider"/>.</item>
    ///     <item>Scans all loaded types for implementations of <see cref="IEventingHandler{TEvent}"/> and registers them as scoped services.</item>
    ///     <item>Registers <see cref="IRegisterEventingHandlersTask"/> as a scoped service for automatic event handler registration.</item>
    ///     </list>
    /// </remarks>
    public static IServiceCollection AddNanoEventing<TProvider>(this IServiceCollection services)
        where TProvider : IEventingProvider
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddNanoConfigSection<EventingOptions>(EventingOptions.SectionName, out var options);

        if (options is null)
        {
            throw new InvalidOperationException($"Configuration section '{EventingOptions.SectionName}' could not be loaded.");
        }

        TProvider.Configure(services, options);

        services
            .AddEventingHandlers()
            .AddScoped<IRegisterEventingHandlersTask, RegisterEventingHandlersTask>();

        return services;
    }


    private static IServiceCollection AddEventingHandlers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var eventHandlerTypes = TypeCache
            .GetAllTypes()
            .SelectMany(x => x.GetInterfaces(), (x, y) => new
            {
                Type = x,
                GenericType = y
            })
            .Where(x =>
                x.Type is { IsAbstract: false, IsGenericType: false } &&
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
            var serviceType = eventHandlerType?.GenericType;
            var implementationType = eventHandlerType?.Type;

            if (serviceType == null || implementationType == null)
            {
                continue;
            }

            services
                .AddScoped(serviceType, implementationType);
        }

        return services;
    }
}