using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Extensions;
using Nano.Data.Eventing.Models;
using Nano.Eventing.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Eventing.Interceptors;

internal sealed class EntityEventingSaveChangesInterceptor(IEventing eventing)
    : SaveChangesInterceptor, IDbTransactionInterceptor
{
    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    private readonly List<(Type Type, EntityEvent Event, object Entity)> pendingEvents = [];

    private readonly List<EntityEvent> deferredEvents = [];

    public ValueTask<InterceptionResult<DbTransaction>> TransactionStartingAsync(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();

        return new ValueTask<InterceptionResult<DbTransaction>>(result);
    }

    public InterceptionResult<DbTransaction> TransactionStarting(DbConnection connection, TransactionStartingEventData eventData, InterceptionResult<DbTransaction> result)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();

        return result;
    }

    public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        this.PublishDeferredEventsAsync()
            .GetAwaiter()
            .GetResult();
    }

    public async Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        await this.PublishDeferredEventsAsync(cancellationToken);
    }

    public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();
    }

    public Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();

        return Task.CompletedTask;
    }

    public void TransactionFailed(DbTransaction transaction, TransactionErrorEventData eventData)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();
    }

    public Task TransactionFailedAsync(DbTransaction transaction, TransactionErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(eventData);

        this.deferredEvents
            .Clear();

        return Task.CompletedTask;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context == null)
        {
            throw new NullReferenceException(nameof(eventData.Context));
        }

        this.PreSaveEntityEvents(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context == null)
        {
            throw new NullReferenceException(nameof(eventData.Context));
        }

        this.PreSaveEntityEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context == null)
        {
            throw new NullReferenceException(nameof(eventData.Context));
        }

        this.PublishEntityEvents(eventData.Context)
            .GetAwaiter()
            .GetResult();

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        if (eventData.Context == null)
        {
            throw new NullReferenceException(nameof(eventData.Context));
        }

        await this.PublishEntityEvents(eventData.Context, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }


    private void PreSaveEntityEvents(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var graphHydrator = new EntityGraphHydrator(dbContext);
        var entityEventingModel = EntityEventingModelCache.GetOrCreate(dbContext);

        var directEntities = new HashSet<(Type type, CompositeKey key)>();

        var entries = dbContext.ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted && entityEventingModel.ReversePlans.ContainsKey(x.Entity.GetType()))
            .ToArray();

        foreach (var entry in entries)
        {
            var type = entry.Metadata.ClrType;

            var reversePlans = entityEventingModel.ReversePlans
                .GetPlans(type);

            graphHydrator
                .HydrateEntry(entry);

            var changedProperties = GetChangedProperties(entry, reversePlans);

            if (entry.State == EntityState.Modified && changedProperties.Length == 0)
            {
                continue;
            }

            foreach (var plan in reversePlans)
            {
                var shouldTrigger = entry.State != EntityState.Modified || plan.WatchedProperties
                    .Any(x => changedProperties.Contains(x));

                if (!shouldTrigger)
                {
                    continue;
                }

                var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

                if (plan.NavigationSteps.Count == 0)
                {
                    if (!directEntities.Add((plan.RootType, entry.GetCompositeKey())))
                    {
                        continue;
                    }

                    graphHydrator
                        .HydrateEntry(entry);

                    graphHydrator
                        .HydratePublish(entry, plan.RootType, visited);

                    this.pendingEvents
                        .Add((plan.RootType, GetEntityEvent(entry, plan.RootType), entry.Entity));
                }
                else
                {
                    var rootEntries = new List<EntityEntry> { entry };

                    foreach (var navigationStep in plan.NavigationSteps.Reverse())
                    {
                        var nextEntries = new List<EntityEntry>();

                        foreach (var rootEntry in rootEntries)
                        {
                            var entityEntries = graphHydrator
                                .HydratePublishReverse(rootEntry, navigationStep);

                            nextEntries
                                .AddRange(entityEntries);
                        }

                        rootEntries = nextEntries;

                        if (rootEntries.Count == 0)
                        {
                            break;
                        }
                    }

                    foreach (var rootEntry in rootEntries)
                    {
                        if (!directEntities.Add((plan.RootType, rootEntry.GetCompositeKey())))
                        {
                            continue;
                        }

                        graphHydrator
                            .HydratePublish(rootEntry, plan.RootType, visited);

                        this.pendingEvents
                            .Add((plan.RootType, GetEntityEvent(rootEntry, plan.RootType), rootEntry.Entity));
                    }
                }
            }
        }
    }
    private async Task PublishEntityEvents(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityEventingModel = EntityEventingModelCache.GetOrCreate(dbContext);

        var entries = dbContext.ChangeTracker
            .Entries()
            .ToList();

        var lookup = entries
            .ToLookup(x => (Type: x.Entity.GetType(), Key: x.GetPrimaryKeyValue()), x => x);

        var events = new List<EntityEvent>();

        try
        {
            foreach (var pending in this.pendingEvents)
            {
                var entry = lookup[(pending.Type, pending.Event.Id)].FirstOrDefault();

                if (pending.Event.State is "Added" or "Modified")
                {
                    if (entry == null)
                    {
                        continue;
                    }

                    var accessors = entityEventingModel.Accessors
                        .Get(pending.Type);

                    foreach (var accessor in accessors)
                    {
                        pending.Event.Data[accessor.Name] = accessor.Accessor(entry.Entity);
                    }
                }

                events
                    .Add(pending.Event);
            }
        }
        finally
        {
            this.pendingEvents.Clear();
        }

        if (dbContext.Database.CurrentTransaction != null)
        {
            this.deferredEvents
                .AddRange(events);

            return;
        }

        await this.SendEventsAsync(events, cancellationToken);
    }
    private async Task PublishDeferredEventsAsync(CancellationToken cancellationToken = default)
    {
        var events = Coalesce(this.deferredEvents);

        this.deferredEvents
            .Clear();

        await this.SendEventsAsync(events, cancellationToken);
    }
    private async Task SendEventsAsync(IEnumerable<EntityEvent> events, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var entityEvent in events)
        {
            await this.eventing
                .PublishAsync(entityEvent, entityEvent.TypeName, cancellationToken);
        }
    }

    private static List<EntityEvent> Coalesce(IReadOnlyList<EntityEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        var result = new List<EntityEvent>();
        var indexes = new Dictionary<(string TypeName, object Id), int>();

        foreach (var entityEvent in events)
        {
            var key = (entityEvent.TypeName, entityEvent.Id);

            if (!indexes.TryGetValue(key, out var index))
            {
                indexes[key] = result.Count;
                result.Add(entityEvent);

                continue;
            }

            var previous = result[index];

            if (previous.State == "Deleted")
            {
                indexes[key] = result.Count;
                result.Add(entityEvent);

                continue;
            }

            if (entityEvent.State == "Deleted")
            {
                if (previous.State == "Added")
                {
                    result[index] = null!;
                    indexes.Remove(key);
                }
                else
                {
                    result[index] = entityEvent;
                }

                continue;
            }

            result[index] = previous.State == "Added"
                ? new EntityEvent(entityEvent.Id, entityEvent.TypeName, "Added") { Data = entityEvent.Data }
                : entityEvent;
        }

        return result
            .Where(x => x != null)
            .ToList();
    }
    private static string[] GetChangedProperties(EntityEntry entry, IReadOnlyCollection<ReversePlan> reversePlans)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(reversePlans);

        if (entry.State != EntityState.Modified || reversePlans.Count == 0)
        {
            return [];
        }

        var relevantProperties = reversePlans
            .SelectMany(p => p.WatchedProperties)
            .ToHashSet();

        var changedProperties = entry.Properties
            .Where(p =>
                relevantProperties.Contains(p.Metadata.Name) &&
                !Equals(p.OriginalValue, p.CurrentValue))
            .Select(p => p.Metadata.Name)
            .ToHashSet();

        if (changedProperties.Count == 0)
        {
            return [];
        }

        return changedProperties
            .ToArray();
    }
    private static EntityEvent GetEntityEvent(EntityEntry entry, MemberInfo type)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(type);

        var state = entry.State is EntityState.Unchanged or EntityState.Detached
            ? nameof(EntityState.Modified)
            : entry.State.ToString();

        return entry.Entity switch
        {
            IEntityIdentity<int> x => new EntityEvent(x.Id, type.Name, state),
            IEntityIdentity<long> x => new EntityEvent(x.Id, type.Name, state),
            IEntityIdentity<Guid> x => new EntityEvent(x.Id, type.Name, state),
            IEntityIdentity<string> x => new EntityEvent(x.Id, type.Name, state),
            _ => throw new ArgumentOutOfRangeException(nameof(entry.Entity), entry.Entity, "Argument out of range.")
        };
    }
}