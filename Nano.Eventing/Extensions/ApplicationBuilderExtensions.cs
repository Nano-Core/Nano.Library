using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nano.Eventing.Handlers;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Attributes;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Helpers;

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
    /// <param name="useEntityEventHandlers">Use entity event handlers.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseEventHandlers(this IApplicationBuilder applicationBuilder, bool useEntityEventHandlers)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

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
                var genericHandlerType = x.GenericType;
                var eventType = genericHandlerType
                    .GetGenericArguments()[0];

                var eventing = applicationBuilder.ApplicationServices
                    .GetRequiredService<IEventing>();

                eventing
                    .GetType()
                    .GetMethod(nameof(IEventing.SubscribeAsync))?
                    .MakeGenericMethod(eventType)
                    .Invoke(eventing, 
                    [
                        applicationBuilder.ApplicationServices,
                        string.Empty,
                        CancellationToken.None
                    ]);
            });

        if (useEntityEventHandlers)
        {
            TypesHelper.GetAllTypes()
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
                        .Invoke(eventing, [
                            applicationBuilder.ApplicationServices,
                            x.Name,
                            CancellationToken.None
                        ]);
                });
        }

        return applicationBuilder;
    }
}