using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Models;
using Nano.Eventing.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Eventing.Interceptors;

// BUG: TEST: Try with creating an ExampleNavigation with a list of Examples, and see that all Examples are published
// BUG: TEST: try with triple dependency, to verify we can handle deeper dependencies
// BUG: TEST: Triggers (we want update events to have trigger changes included in the event - newest value)
// BUG: TEST: Soft Delete (we want to publish a deleted state, so subscriber can decide soft delete or not)
// BUG: TEST: Reverse update. Update ExampleNavigation and see Example is published

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

        var hydrator = new EntityGraphHydrator(dbContext);
        var entityEventingModel = EntityEventingModelCache.GetOrCreate(dbContext);

        var directEntities = new HashSet<(Type type, CompositeKey key)>();

        var entries = dbContext.ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted && entityEventingModel.ReversePlans.ContainsKey(x.Entity.GetType()))
            .ToArray();

        foreach (var entry in entries)
        {
            var type = entry.Entity
                .GetType();

            if (!entityEventingModel.ReversePlans.TryGetValue(type, out var plans))
            {
                continue;
            }

            hydrator
                .HydrateEntry(entry);

            // BUG: what if we have nested properties that has changed. Loading something with include and change nested and then update
            // This seems right for reverse because if this entry hasn't changed any relevant properties, but for update through another like Customer -> profile change
            // would abort here, no?

            HashSet<string>? changedProperties = null;

            if (entry.State == EntityState.Modified)
            {
                var relevantProperties = plans
                    .SelectMany(p => p.WatchedProperties)
                    .ToHashSet();

                changedProperties = entry.Properties
                    .Where(p =>
                        relevantProperties.Contains(p.Metadata.Name) &&
                        !Equals(p.OriginalValue, p.CurrentValue))
                    .Select(p => p.Metadata.Name)
                    .ToHashSet();

                if (changedProperties.Count == 0)
                    continue;
            }

            foreach (var plan in plans)
            {
                bool shouldTrigger;

                if (entry.State == EntityState.Modified)
                {
                    shouldTrigger = plan.WatchedProperties
                        .Any(p => changedProperties!.Contains(p));
                }
                else
                {
                    shouldTrigger = true;
                }

                if (!shouldTrigger)
                    continue;


                var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

                // Direct entity
                if (plan.Path.Count == 0)
                {
                    hydrator.HydrateEntry(entry);

                    hydrator.HydratePublish(entry, plan.RootType, visited);

                    var key = GetKey(entry);

                    if (!directEntities.Add((plan.RootType, key)))
                        continue;

                    var entityEvent = GetEntityEvent(entry, plan.RootType);

                    this.pendingEvents.Add((plan.RootType, entityEvent, entry.Entity));
                }
                else
                {
                    var rootEntries = new List<EntityEntry> { entry };

                    foreach (var step in plan.Path.Reverse())
                    {
                        var nextEntries = new List<EntityEntry>();

                        foreach (var entry2 in rootEntries)
                        {
                            var fk = step.ForeignKey;

                            if (!step.IsOnDependent)
                            {
                                if (entry2.Metadata != fk.DeclaringEntityType)
                                    continue;

                                nextEntries.AddRange(GetRelatedEntries(entry2, fk, goToPrincipal: true));
                            }
                            else
                            {
                                if (entry2.Metadata != fk.PrincipalEntityType)
                                    continue;

                                nextEntries.AddRange(GetRelatedEntries(entry2, fk, goToPrincipal: false));
                            }
                        }

                        rootEntries = nextEntries;

                        if (rootEntries.Count == 0)
                        {
                            break;
                        }
                    }

                    foreach (var rootEntry in rootEntries)
                    {
                        var key = GetKey(rootEntry);

                        if (!directEntities.Add((plan.RootType, key)))
                            continue;

                        hydrator.HydratePublish(rootEntry, plan.RootType, visited);

                        var entityEvent = GetEntityEvent(rootEntry, plan.RootType);

                        this.pendingEvents.Add((plan.RootType, entityEvent, rootEntry.Entity));
                    }
                }
            }
        }
    }
    private static IEnumerable<EntityEntry> GetRelatedEntries(EntityEntry entry, IForeignKey fk, bool goToPrincipal)
    {
        if (goToPrincipal)
        {
            var navigation = entry.Navigations
                .FirstOrDefault(n => n.Metadata is INavigation nav && nav.ForeignKey == fk);

            if (navigation?.CurrentValue == null)
                yield break;

            yield return entry.Context.Entry(navigation.CurrentValue);
        }
        else
        {
            foreach (var navigation in entry.Metadata.GetNavigations()
                         .Where(n => n.ForeignKey == fk))
            {
                var navEntry = entry.Navigation(navigation.Name);

                if (!navEntry.IsLoaded)
                    navEntry.Load();

                if (navEntry.CurrentValue is IEnumerable<object> collection)
                {
                    foreach (var item in collection)
                        yield return entry.Context.Entry(item);
                }
                else if (navEntry.CurrentValue != null)
                {
                    yield return entry.Context.Entry(navEntry.CurrentValue);
                }
            }
        }
    }


    private CompositeKey GetKey(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var primaryKey = entry.Metadata.FindPrimaryKey();

        if (primaryKey == null)
        {
            throw new InvalidOperationException($"Entity type '{entry.Metadata.Name}' does not have a primary key.");
        }

        var values = primaryKey.Properties
            .Select(p => entry.Property(p.Name).CurrentValue)
            .ToArray();

        return new CompositeKey(values!);
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

    private async Task PublishEntityEvents(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityEventingModel = EntityEventingModelCache.GetOrCreate(dbContext);

        var entries = dbContext.ChangeTracker.Entries()
            .ToList();

        var lookup = entries
            .ToLookup(x => (Type: x.Entity.GetType(), Key: GetPrimaryKeyValue(x)), x => x);

        try
        {
            var tasks = this.pendingEvents
                .Select(x => this.ProcessEvent(dbContext, entityEventingModel, x.Type, x.Event, lookup!, cancellationToken));

            await Task.WhenAll(tasks);
        }
        finally
        {
            this.pendingEvents.Clear();
        }
    }
    private Task ProcessEvent(DbContext dbContext, EntityEventingModel entityEventingModel, Type entityType, EntityEvent entityEvent, ILookup<(Type Type, object Key), EntityEntry> lookup, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entityEventingModel);
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(entityEvent);

        var key = (entityType, entityEvent.Id);

        var entry = lookup[key].FirstOrDefault();

        if (entry == null && entityEvent.State != "Deleted")
        {
            return Task.CompletedTask;
        }

        if (entityEvent.State is "Added" or "Modified")
        {
            if (!entityEventingModel.Metadata.TryGetValue(entityType, out var metadata))
            {
                return Task.CompletedTask;
            }

            var data = new Dictionary<string, object?>();

            foreach (var path in metadata.Properties)
            {
                var accessorKey = (entityType, path);

                if (!entityEventingModel.Accessors.TryGetValue(accessorKey, out var accessor))
                {
                    throw new InvalidOperationException(
                        $"Accessor not found for type '{entityEvent.TypeName}' and path '{path}'");
                }

                var value = accessor(entry!.Entity);
                data[path.Split('.').Last()] = value;
            }

            entityEvent.Data = data;
        }

        Console.WriteLine(JsonConvert.SerializeObject(entityEvent, Formatting.Indented)); // TODO: remove

        return this.eventing.PublishAsync(entityEvent, entityEvent.TypeName, cancellationToken);
    }
    private static object? GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();

        return key?.Properties
            .Select(p => entry.Property(p.Name).CurrentValue)
            .FirstOrDefault();
    }
}