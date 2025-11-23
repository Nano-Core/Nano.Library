using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Eventing.Abstractions;
using Nano.Eventing.Abstractions.Config;

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

        var eventingProvider = new TProvider();

        eventingProvider
            .Configure(services);

        services
            .AddEventingHandlers();

        return services;
    }


    private static IServiceCollection AddEventingHandlers(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        TypesHelper
            .GetAllTypes()
            .SelectMany(x => x.GetInterfaces(), (x, y) => new
            {
                Type = x,
                GenericType = y
            })
            .Where(x =>
                !x.Type.IsAbstract &&
                x.Type.IsTypeOf(typeof(IEventingHandler<>)))
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
}