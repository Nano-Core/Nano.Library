using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.TypeMap;
using Nano.Data.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nano.Data;

internal class EntityHydrator
{
    protected static readonly ConcurrentDictionary<Type, Func<DbContext, IEntityType, object[], object?>> LoaderCache = new();
}

internal class EntityHydrator<TIdentity>(BaseDbContext<TIdentity> dbContext) : EntityHydrator
    where TIdentity : IEquatable<TIdentity>
{
    private readonly BaseDbContext<TIdentity> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    internal readonly HashSet<object> hydratedEntities = new(ReferenceEqualityComparer.Instance);

    internal void HydrateEntry(EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        if (!this.hydratedEntities.Add(entityEntry.Entity))
            return;

        if (entityEntry.HasOriginalValues())
            return;

        if (entityEntry.State == EntityState.Added)
        {
            HydrateAddedEntity(entityEntry);
            return;
        }

        HydrateExistingEntity(entityEntry);
    }

    internal void HydrateReverseDependencies(EntityEntry changedEntry)
    {
        var model = EntityEventingModelCache.GetOrCreate(this.dbContext);

        var changedType = changedEntry.Entity.GetType();

        if (!model.ReverseBindings.TryGetValue(changedType, out var map))
            return;

        foreach (var (rootType, bindings) in map)
        {
            // group bindings per FK (this is the batching)
            var grouped = bindings.GroupBy(b => b.ForeignKey);

            foreach (var group in grouped)
            {
                var fk = group.Key;
                IReadOnlyList<IProperty> properties;// = fk.Properties;
                if (fk.DeclaringEntityType.ClrType == rootType)
                    properties = fk.Properties; // dependent side
                else
                    properties = fk.PrincipalKey.Properties; // principal side

                var queryFactory = ForeignKeyQueryCache.Get(rootType, properties);

                var values = properties
                    .Select(p => changedEntry.Property(p.Name).CurrentValue)
                    .ToArray();

                if (values.Any(v => v == null))
                    continue;

                var query = queryFactory(this.dbContext, values!);

                var results = query.Cast<object>().ToList();

                foreach (var entity in results)
                {
                    var entry = this.dbContext.Entry(entity);

                    if (entry.State == EntityState.Detached)
                    {
                        entry = this.dbContext
                            .Attach(entity);
                    }

                    Hydrate(entry);
                }
            }
        }
    }


    internal void Hydrate(EntityEntry rootEntry)
    {
        ArgumentNullException.ThrowIfNull(rootEntry);

        GraphTraverser.TraverseGraph(this.dbContext, rootEntry, new GraphTraverserOptions
        {
            GetPaths = entry =>
            {
                var model = EntityEventingModelCache.GetOrCreate(entry.Context);
                var type = entry.Entity.GetType();

                return model.EntityMap.TryGetValue(type, out var metadata)
                    ? metadata.Properties
                    : Enumerable.Empty<string>();
            },

            LoadNavigations = true,
            IncludeCollections = true, // BUG: why is this true. Maybe we can simply all this a bit.

            ShouldTraverseNavigation = (_, __) => true,

            OnEntityVisited = (entry, path) =>
            {
                ProcessPublishPaths(entry, path);
                ProcessAuditable(entry, path);
            }
        });
    }
    private void ProcessPublishPaths(EntityEntry entry, string? path)
    {
        ArgumentNullException.ThrowIfNull(entry);

        this.HydrateEntry(entry);
    }
    private void ProcessAuditable(EntityEntry entry, string? path)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.Entity is IEntityAuditable)
        {
            this.HydrateEntry(entry);
        }
    }



    private void HydrateAddedEntity(EntityEntry entityEntry)
    {
        var entityType = entityEntry.Metadata;

        foreach (var navigation in entityType.GetNavigations())
        {
            if (navigation.IsCollection)
                continue;

            var navEntry = entityEntry.Navigation(navigation.Name);

            if (!navEntry.IsLoaded)
            {
                navEntry.Load();
            }
        }
    }
    private void HydrateExistingEntity(EntityEntry entityEntry)
    {
        var entityType = entityEntry.Metadata;

        var key = entityType.FindPrimaryKey();
        if (key == null)
            return;

        var keyValues = key.Properties
            .Select(p => entityEntry.Property(p.Name).CurrentValue)
            .ToArray();

        var clrType = entityEntry.Entity.GetType();

        var dbEntity = this.LoadEntityNoTracking(clrType, entityType, keyValues!);
        if (dbEntity == null)
            return;

        var dbEntry = this.dbContext.Entry(dbEntity);

        foreach (var property in entityType.GetProperties())
        {
            if (property.IsPrimaryKey() ||
                property.IsShadowProperty() ||
                entityType.FindNavigation(property.Name) != null)
            {
                continue;
            }

            var dbValue = dbEntry.Property(property.Name).CurrentValue;

            entityEntry.OriginalValues[property.Name] = dbValue;

            var isStoreGenerated =
                property.ValueGenerated == ValueGenerated.OnAdd ||
                property.ValueGenerated == ValueGenerated.OnAddOrUpdate ||
                property.GetAfterSaveBehavior() == PropertySaveBehavior.Ignore;

            if (isStoreGenerated)
            {
                entityEntry.Property(property.Name).CurrentValue = dbValue;
            }
        }
    }

    private object? LoadEntityNoTracking(Type clrType, IEntityType entityType, object[] keyValues)
    {
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(keyValues);

        var loaderCache = LoaderCache
            .GetOrAdd(clrType, x =>
            {
                var method = this.dbContext
                    .GetType()
                    .GetMethod(nameof(LoadEntityNoTrackingGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(x);

                return (Func<DbContext, IEntityType, object[], object?>)((ctx, et, kv) => method.Invoke(this, [ctx, et, kv]));
            });

        return loaderCache(this.dbContext, entityType, keyValues);
    }
    private object? LoadEntityNoTrackingGeneric<TEntity>(IEntityType entityType, object[] keyValues)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(keyValues);

        var predicate = EntityKeyPredicateCache.GetPredicate<TEntity>(entityType, keyValues);

        return this.dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefault(predicate);
    }
}