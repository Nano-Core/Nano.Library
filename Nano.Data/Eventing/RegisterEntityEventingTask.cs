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

// BUG: 000: Fix event handlers and dbcontext, registration is wrong

// BUG: 000: Final review of entity eventing with Chat-GPT

// BUG: 000: When no auth things work but it shows IsInRole failures in log. Figure out why and if we can remove it

// BUG: new migrations for all data lessons
// BUG: check if all nullable suppressions properties should just have required keyword

/// <inheritdoc />
internal sealed class RegisterEntityEventingTask(DbContext dbContext, IEventing? eventing = null)
    : IRegisterEntityEventingTask
{
    private readonly DbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IEventing? eventing = eventing;

    /// <inheritdoc />
    public async Task InitializeEntityEventing(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        EntityEventingModelCache.GetOrCreate(dbContext);
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