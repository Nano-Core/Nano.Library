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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Eventing.Interceptors;

internal sealed class EntityEventingSaveChangesInterceptor(IEventing eventing)
    : SaveChangesInterceptor
{
    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    private readonly List<(Type Type, EntityEvent Event, object Entity)> pendingEvents = [];

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        this.PreSaveEntityEvents(eventData.Context!);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        this.PreSaveEntityEvents(eventData.Context!);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        this.PublishEntityEvents(eventData.Context!).GetAwaiter().GetResult();

        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);

        await this.PublishEntityEvents(eventData.Context!, cancellationToken);

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

        try
        {
            var tasks = this.pendingEvents
                .Select(x =>
                {
                    var key = (x.Type, x.Event.Id);
                    var entry = lookup[key].FirstOrDefault();

                    switch (x.Event.State)
                    {
                        case "Added":
                        case "Modified":
                            if (entry == null)
                            {
                                return Task.CompletedTask;
                            }

                            var accessors = entityEventingModel.Accessors
                                .Get(x.Type);

                            foreach (var accessor in accessors)
                            {
                                x.Event.Data[accessor.Name] = accessor.Accessor(entry.Entity);
                            }

                            break;

                        case "Deleted":
                            break;
                    }

                    return this.eventing
                        .PublishAsync(x.Event, x.Event.TypeName, cancellationToken);
                });

            await Task.WhenAll(tasks);
        }
        finally
        {
            this.pendingEvents.Clear();
        }
    }

    private static string[] GetChangedProperties(EntityEntry entry, List<ReversePlan> reversePlans)
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
    private static EntityEvent GetEntityEvent(EntityEntry entry, Type type)
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