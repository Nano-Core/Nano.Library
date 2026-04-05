using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Eventing.TypeMap;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Eventing;

/// <inheritdoc />
internal sealed class RegisterEntityEventingHandlersTask(IEventing? eventing = null)
    : IRegisterEntityEventingHandlersTask
{
    /// <inheritdoc />
    public async Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        if (eventing == null)
        {
            return;
        }

        var dbContext = serviceProvider
            .GetRequiredService<DbContext>();

        var entityMap = EntityEventingTypeMapCache.GetOrCreate(dbContext);

        foreach (var entityType in entityMap)
        {
            var eventType = typeof(EntityEvent);

            var eventHandler = serviceProvider
                .GetRequiredService<IEventingHandler<EntityEvent>>();

            var subscribeMethod = eventing
                .GetType()
                .GetMethod(nameof(IEventing.SubscribeAsync));

            subscribeMethod?
                .MakeGenericMethod(eventType)
                .Invoke(eventing, [eventHandler, entityType.Key.Name, null, CancellationToken.None]);
        }
    }
}