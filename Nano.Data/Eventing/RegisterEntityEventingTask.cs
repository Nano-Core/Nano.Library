using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Eventing;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Eventing;

/// <inheritdoc />
internal sealed class RegisterEntityEventingTask(DbContext dbContext, IEventing? eventing = null)
    : IRegisterEntityEventingTask
{
    /// <inheritdoc />
    public async Task InitializeEntityEventing(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var a = EntityEventingModelCache.GetOrCreate(dbContext);
    }

    /// <inheritdoc />
    public async Task RegisterEntityEventHandlers(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await Task.CompletedTask;

        if (eventing == null)
        {
            return;
        }

        var eventType = typeof(EntityEvent);

        var subscribeMethod = eventing
            .GetType()
            .GetMethod(nameof(IEventing.SubscribeAsync))!
            .MakeGenericMethod(eventType);

        var eventHandler = serviceProvider
            .GetRequiredService<IEventingHandler<EntityEvent>>();

        var entityTypeNames = dbContext.Model
            .GetEntityTypes()
            .Where(x => x.ClrType.GetCustomAttribute<SubscribeAttribute>() != null)
            .Select(x => x.Name)
            .ToArray();

        foreach (var entityTypeName in entityTypeNames)
        {
            subscribeMethod
                .Invoke(eventing, [eventHandler, entityTypeName, null, CancellationToken.None]);
        }
    }
}