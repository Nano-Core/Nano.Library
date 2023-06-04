using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Attributes;
using Nano.Eventing.Handlers;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;

namespace Nano.Eventing.Extensions;

/// <summary>
/// Application Builder Extensions.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Eventing subscribers to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseEventHandlers(this IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
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
                var genericHandlerType = x.GenericType;
                var eventType = genericHandlerType
                    .GetGenericArguments()[0];

                var eventing = applicationBuilder.ApplicationServices
                    .GetRequiredService<IEventing>();

                eventing
                    .GetType()
                    .GetMethod(nameof(IEventing.SubscribeAsync))?
                    .MakeGenericMethod(eventType)
                    .Invoke(eventing, new object[]
                    {
                        serviceProvider,
                        string.Empty,
                        CancellationToken.None
                    });
            });

        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x =>
                !x.IsAbstract &&
                !x.Name.EndsWith("Proxy") &&
                x.GetCustomAttributes<SubscribeAttribute>().Any())
            .GroupBy(x => x.FullName)
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null)
            .ToList()
            .ForEach(x =>
            {
                var eventType = typeof(EntityEvent);

                var eventing = applicationBuilder.ApplicationServices
                    .GetRequiredService<IEventing>();

                eventing
                    .GetType()
                    .GetMethod(nameof(IEventing.SubscribeAsync))?
                    .MakeGenericMethod(eventType)
                    .Invoke(eventing, new object[]
                    {
                        serviceProvider,
                        x.Name,
                        CancellationToken.None
                    });
            });

        return applicationBuilder;
    }
}