using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Eventing.Abstractions;
using System;
using System.Linq;
using System.Threading;

namespace Nano.App.Extensions;

internal static class ApplicationBuilderExtensions
{
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

    internal static IApplicationBuilder UseDbMigrations(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        using var scope = applicationBuilder.ApplicationServices
            .CreateScope();

        var dbMigrationTask = scope.ServiceProvider
            .GetService<IDbMigrationTask>();

        dbMigrationTask?
            .MigrateAndSeedAsync()
            .GetAwaiter()
            .GetResult();

        return applicationBuilder;
    }
}