using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Eventing.TypeMap;
using Newtonsoft.Json;

namespace Nano.Data.Eventing.Interceptors;

// BUG: TEST: Triggers (we want update events to have trigger changes included in the event - newest value)
// BUG: TEST: Soft Delete (we want to publish a deleted state, so subscriber can decide soft delete or not)
// BUG: TEST: Reverse update. Update ExampleNavigation and see Example is published

internal sealed class EntityEventingSaveChangesInterceptor(IEventing eventing)
    : SaveChangesInterceptor
{
    private readonly IEventing eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));

    private readonly List<EntityEvent> pendingEvents = [];

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

        var model = EntityEventingModelCache.GetOrCreate(dbContext);

        var entries = dbContext.ChangeTracker.Entries()
            .Where(x => model.EntityMap.ContainsKey(x.Entity.GetType()) && x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entry in entries)
        {
            var entityEvent = GetEntityEvent(entry, entry.Entity.GetType());

            if (entry.State == EntityState.Modified)
            {
                var hasRelevantChanges = false;
                var metadata = model.EntityMap[entry.Entity.GetType()];

                foreach (var path in metadata.Properties)
                {
                    if (TryEvaluatePath(entry, path, out var original, out var current))
                    {
                        if (!Equals(original, current))
                        {
                            hasRelevantChanges = true;
                            break;
                        }
                    }
                }

                if (!hasRelevantChanges)
                {
                    continue;
                }
            }

            this.pendingEvents
                .Add(entityEvent);
        }
    }
    private async Task PublishEntityEvents(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var model = EntityEventingModelCache.GetOrCreate(dbContext);

        try
        {
            var tasks = this.pendingEvents
                .Select(x => this.ProcessEvent(dbContext, model, x, cancellationToken));

            await Task.WhenAll(tasks);
        }
        finally
        {
            this.pendingEvents
                .Clear();
        }
    }

    private static EntityEvent GetEntityEvent(EntityEntry entry, Type type)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(type);

        var state = entry.State.ToString();

        return entry.Entity switch
        {
            IEntityIdentity<int> e => new EntityEvent(e.Id, type.Name, state),
            IEntityIdentity<long> e => new EntityEvent(e.Id, type.Name, state),
            IEntityIdentity<Guid> e => new EntityEvent(e.Id, type.Name, state),
            IEntityIdentity<string> e => new EntityEvent(e.Id, type.Name, state),
            _ => throw new ArgumentOutOfRangeException(nameof(entry.Entity), entry.Entity, "Argument out of range.")
        };
    }
    private static bool TryEvaluatePath(EntityEntry entry, string path, out object? original, out object? current)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(path);

        var currentEntry = entry;
        var segments = path.Split('.');

        for (var i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];

            var navigation = currentEntry.Metadata
                .FindNavigation(segment);

            if (navigation != null)
            {
                var navEntry = currentEntry
                    .Navigation(segment);

                var nextEntity = navEntry.CurrentValue;

                if (nextEntity == null)
                {
                    original = null;
                    current = null;

                    return false;
                }

                currentEntry = currentEntry.Context
                    .Entry(nextEntity);
            }
            else
            {
                if (i != segments.Length - 1)
                {
                    original = null;
                    current = null;
                    return false;
                }

                var prop = currentEntry
                    .Property(segment);

                original = prop.OriginalValue;
                current = prop.CurrentValue;

                return true;
            }
        }

        original = null;
        current = null;

        return false;
    }
    private Task ProcessEvent(DbContext dbContext, EntityEventingModel entityEventingModel, EntityEvent entityEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entityEventingModel);
        ArgumentNullException.ThrowIfNull(entityEvent);

        if (!entityEventingModel.EntityMap.Keys.ToDictionary(t => t.Name, t => t).TryGetValue(entityEvent.Type, out var entityType))
        {
            return Task.CompletedTask;
        }

        var entry = dbContext.ChangeTracker.Entries()
            .FirstOrDefault(x => x.Entity.GetType() == entityType && Equals(GetPrimaryKeyValue(x), entityEvent.Id));

        if (entry == null && entityEvent.State != "Deleted")
        {
            return Task.CompletedTask;
        }

        switch (entityEvent.State)
        {
            case "Added":
            case "Modified":
                var data = new Dictionary<string, object?>();
                var metadata = entityEventingModel.EntityMap[entityType];

                foreach (var path in metadata.Properties)
                {
                    var key = (entry!.Entity.GetType(), path);

                    if (!entityEventingModel.Accessors.TryGetValue(key, out var accessor))
                    {
                        throw new InvalidOperationException($"Accessor not found for {path}");
                    }

                    var value = accessor(entry.Entity);

                    data[path] = value;
                }

                entityEvent.Data = data!;
                break;
        }

        Console.WriteLine(JsonConvert.SerializeObject(entityEvent, Formatting.Indented)); // BUG: Remove

        return this.eventing
            .PublishAsync(entityEvent, entityEvent.Type, cancellationToken);
    }
    private static object? GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();

        return key?.Properties
            .Select(p => entry.Property(p.Name).CurrentValue)
            .FirstOrDefault();
    }
}