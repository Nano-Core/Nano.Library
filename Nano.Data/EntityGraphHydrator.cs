using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Models;
using Nano.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.Data.Eventing;

namespace Nano.Data;

internal class EntityGraphHydrator(DbContext dbContext)
{
    private static readonly ConcurrentDictionary<Type, Func<DbContext, IEntityType, object[], object?>> loaderCache = new();

    private readonly DbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly HashSet<object> hydratedEntities = new(ReferenceEqualityComparer.Instance);
    private readonly EntityEventingModel entityEventingModel = EntityEventingModelCache.GetOrCreate(dbContext);

    internal void HydrateEntry(EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        if (!this.hydratedEntities.Add(entityEntry.Entity))
        {
            return;
        }

        if (entityEntry.HasOriginalValues())
        {
            return;
        }

        switch (entityEntry.State)
        {
            case EntityState.Added:
            {
                foreach (var navigation in entityEntry.Metadata.GetNavigations())
                {
                    if (navigation.IsCollection)
                    {
                        continue;
                    }

                    var navigationEntry = entityEntry
                        .Navigation(navigation.Name);

                    if (!navigationEntry.IsLoaded)
                    {
                        navigationEntry
                            .Load();
                    }
                }

                break;
            }
            case EntityState.Modified:
            {
                if (entityEntry.Metadata.IsOwned())
                {
                    return;
                }

                var dbEntry = this.LoadDbEntry(entityEntry);

                if (dbEntry == null)
                {
                    break;
                }

                foreach (var property in entityEntry.Metadata.GetProperties())
                {
                    if (property.IsPrimaryKey() || property.IsShadowProperty() || entityEntry.Metadata.FindNavigation(property.Name) != null)
                    {
                        continue;
                    }

                    var dbValue = dbEntry
                        .Property(property.Name).CurrentValue;

                    entityEntry.OriginalValues[property.Name] = dbValue;

                    var isStoreGenerated = property.ValueGenerated == ValueGenerated.OnAdd || property.ValueGenerated == ValueGenerated.OnAddOrUpdate || property.GetAfterSaveBehavior() == PropertySaveBehavior.Ignore;

                    if (isStoreGenerated)
                    {
                        entityEntry
                            .Property(property.Name).CurrentValue = dbValue;
                    }
                }

                break;
            }
            case EntityState.Deleted:
            {
                if (entityEntry.Metadata.IsOwned())
                {
                    return;
                }

                var dbEntry = this.LoadDbEntry(entityEntry);

                if (dbEntry == null)
                {
                    break;
                }

                foreach (var property in entityEntry.Metadata.GetProperties())
                {
                    if (property.IsPrimaryKey() || property.IsShadowProperty() || entityEntry.Metadata.FindNavigation(property.Name) != null)
                    {
                        continue;
                    }

                    var dbValue = dbEntry
                        .Property(property.Name).CurrentValue;

                    entityEntry.OriginalValues[property.Name] = dbValue;
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(entityEntry.State), entityEntry.State, "Argument out of range.");
        }
    }

    internal void HydrateAudit(EntityEntry entry, HashSet<object> visited)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(visited);

        if (!visited.Add(entry.Entity))
        {
            return;
        }

        if (entry.Entity is IEntityAuditable)
        {
            this.HydrateEntry(entry);
        }

        var navigations = entry.Metadata
            .GetNavigations();

        foreach (var navigation in navigations)
        {
            var navigationEntry = entry
                .Navigation(navigation.Name);

            var currentValue = navigationEntry.CurrentValue;

            if (currentValue == null)
            {
                continue;
            }

            if (navigationEntry.Metadata.IsCollection)
            {
                foreach (var entity in (IEnumerable)currentValue)
                {
                    var childEntry = this.dbContext
                        .Entry(entity);

                    this.HydrateAudit(childEntry, visited);
                }
            }
            else
            {
                var childEntry = this.dbContext
                    .Entry(currentValue);

                this.HydrateAudit(childEntry, visited);
            }
        }
    }

    internal void HydratePublish(EntityEntry entry, Type rootType, HashSet<object> visited)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(visited);

        if (!visited.Add(entry.Entity))
        {
            return;
        }

        var navigations = this.entityEventingModel.Navigations
            .GetNavigations(rootType, entry.Metadata.ClrType);

        if (navigations.Count == 0)
        {
            return;
        }

        foreach (var navigationName in navigations)
        {
            var navigationEntry = entry
                .Navigation(navigationName);

            if (navigationEntry.Metadata.IsCollection)
            {
                continue;
            }

            if (!navigationEntry.IsLoaded)
            {
                navigationEntry
                    .Load();
            }

            var currentValue = navigationEntry.CurrentValue;

            if (currentValue == null)
            {
                continue;
            }

            var childEntry = this.dbContext
                .Entry(currentValue);

            this.HydratePublish(childEntry, rootType, visited);
        }
    }

    internal IEnumerable<EntityEntry> HydratePublishReverse(EntityEntry entry, NavigationStep navigationStep)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(navigationStep);

        if (navigationStep.IsOnDependent)
        {
            if (entry.State == EntityState.Added)
            {
                yield break;
            }

            if (entry.Metadata != navigationStep.ForeignKey.PrincipalEntityType)
            {
                yield break;
            }

            var navigations = entry.Metadata
                .GetNavigations()
                .Where(x => x.ForeignKey == navigationStep.ForeignKey);

            foreach (var navigation in navigations)
            {
                var navigationEntry = entry
                    .Navigation(navigation.Name);

                if (!navigationEntry.IsLoaded)
                {
                    navigationEntry
                        .Load();
                }

                if (navigationEntry.CurrentValue is IEnumerable<object> collection)
                {
                    foreach (var item in collection)
                    {
                        yield return entry.Context
                            .Entry(item);
                    }
                }
                else if (navigationEntry.CurrentValue != null)
                {
                    yield return entry.Context
                        .Entry(navigationEntry.CurrentValue);
                }
            }
        }
        else
        {
            if (entry.Metadata != navigationStep.ForeignKey.DeclaringEntityType)
            {
                yield break;
            }

            var navigation = entry.Navigations
                .FirstOrDefault(n => n.Metadata is INavigation nav && nav.ForeignKey == navigationStep.ForeignKey);

            if (navigation?.CurrentValue == null)
            {
                yield break;
            }

            yield return entry.Context
                .Entry(navigation.CurrentValue);
        }
    }


    private EntityEntry? LoadDbEntry(EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey();

        if (primaryKey == null)
        {
            return null;
        }

        var keyValues = primaryKey.Properties
            .Select(x => entityEntry.Property(x.Name).CurrentValue)
            .ToArray();

        var clrType = entityEntry.Entity
            .GetType();

        var dbEntity = this.LoadEntityNoTracking(clrType, entityEntry.Metadata, keyValues!);

        if (dbEntity == null)
        {
            return null;
        }

        var dbEntry = this.dbContext
            .Entry(dbEntity);

        return dbEntry;
    }
    private object? LoadEntityNoTracking(Type clrType, IEntityType entityType, object[] keyValues)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(keyValues);

        var cache = EntityGraphHydrator.loaderCache.GetOrAdd(clrType, x =>
        {
            var method = typeof(EntityGraphHydrator)
                .GetMethod(nameof(LoadEntityNoTrackingGeneric), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(x);

            return (Func<DbContext, IEntityType, object[], object?>)method
                .CreateDelegate(typeof(Func<DbContext, IEntityType, object[], object?>));
        });

        return cache(this.dbContext, entityType, keyValues);
    }
    private static object? LoadEntityNoTrackingGeneric<TEntity>(DbContext dbContext, IEntityType entityType, object[] keyValues)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(keyValues);

        var predicate = EntityKeyPredicateCache.GetPredicate<TEntity>(entityType, keyValues);

        return dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefault(predicate);
    }
}