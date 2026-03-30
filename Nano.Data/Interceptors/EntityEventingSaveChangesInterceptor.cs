using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Eventing.Extensions;
using Nano.Eventing.Abstractions;

namespace Nano.Data.Interceptors;

// BUG: ENTITY EVENT: Entity Event Map
// 1. Make a map of Publish Attributes and their property names.
// 2. When SaveChanges then check if any property names are affected (e.g. User.IdentityUser.Email is changed, then User needs to be fetched and published)
// NB: Maybe make startup validation of Publish properties and use of include ???
// also we need to detect if there are changes compared to original values, because only then we should publish update events
// Should use property values from entities when publishing not pre-save. Move all set properites to post save in publish except setting Id, CreatedAt and State
// Test with Triggers

internal sealed class EntityEventingSaveChangesInterceptor(IEventing eventing) : SaveChangesInterceptor
{
    private ConcurrentQueue<EntityEvent> pendingEvents = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        this.PreSaveEntityEvents(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        this.PreSaveEntityEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        this.PublishEntityEvents().Wait();

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await this.PublishEntityEvents(cancellationToken)
            .ConfigureAwait(false);

        return await base.SavedChangesAsync(eventData, result, cancellationToken)
            .ConfigureAwait(false);
    }


    private void PreSaveEntityEvents(DbContext? dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityEvents = dbContext.ChangeTracker
            .GetPendingEntityEvents();

        foreach (var entityEvent in entityEvents)
        {
            this.pendingEvents
                .Enqueue(entityEvent);
        }
    }
    private async Task PublishEntityEvents(CancellationToken cancellationToken = default)
    {
        try
        {
            while (!this.pendingEvents.IsEmpty)
            {
                var success = this.pendingEvents
                    .TryDequeue(out var entityEvent);

                if (success && entityEvent != null)
                {
                    await eventing
                        .PublishAsync(entityEvent, entityEvent.Type, cancellationToken);
                }
            }
        }
        finally
        {
            this.pendingEvents
                .Clear();

            this.pendingEvents = [];
        }
    }
}