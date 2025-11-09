using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Helpers;
using System;
using System.Linq;
using System.Threading;
using Nano.Eventing.Interfaces;
using Nano.Models.Extensions;

namespace Nano.App.Extensions;

/// <summary>
/// Application Builder Extensions.
/// </summary>
internal static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="IHttpContextAccessor"/> middleware, and initializes the current <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpContextAccessor(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var httpContextAccessor = applicationBuilder.ApplicationServices
            .GetRequiredService<IHttpContextAccessor>();

        HttpContextAccessor.Configure(httpContextAccessor);

        return applicationBuilder;
    }

    /// <summary>
    /// Adds Eventing hadlers to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="useEntityEventHandler">Whether to use entity event handler.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseEventHandlers(this IApplicationBuilder applicationBuilder, bool useEntityEventHandler)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var eventing = applicationBuilder.ApplicationServices
            .GetService<IEventing>();

        if (eventing == null)
        {
            return applicationBuilder;
        }

        var eventHandlerTypes = TypesHelper.GetAllTypes()
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
            .Select(x => x.FirstOrDefault()?.GenericType)
            .Where(x => x != null);

        foreach (var type in eventHandlerTypes)
        {
            var eventType = type
                .GetGenericArguments()[0];

            var routing = string.Empty;

            if (eventType == typeof(EntityEvent))
            {
                if (!useEntityEventHandler)
                {
                    continue;
                }

                routing = type.Name;
            }

            eventing
                .GetType()
                .GetMethod(nameof(IEventing.SubscribeAsync))?
                .MakeGenericMethod(eventType)
                .Invoke(eventing,
                [
                    applicationBuilder.ApplicationServices,
                    routing,
                    CancellationToken.None
                ]);
        }

        return applicationBuilder;
    }
}