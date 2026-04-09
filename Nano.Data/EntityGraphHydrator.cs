using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing;
using Nano.Data.Eventing.Models;
using Nano.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nano.Data;

internal class EntityGraphHydrator(DbContext dbContext)
{
    private readonly HashSet<object> hydratedEntities = new(ReferenceEqualityComparer.Instance);
    private readonly DbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private static readonly ConcurrentDictionary<Type, Func<DbContext, IEntityType, object[], object?>> LoaderCache = new();

    internal void HydrateAudit(EntityEntry entry, HashSet<object> visited)
    {
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

    internal void HydratePublish(EntityEntry entry, HashSet<object> visited)
    {
        if (!visited.Add(entry.Entity))
        {
            return;
        }

        var model = EntityEventingModelCache.GetOrCreate(this.dbContext);

        var success = model.Metadata.TryGetValue(entry.Metadata.ClrType, out _);

        if (!success)
        {
            return;
        }

        this.HydrateEntry(entry);

        var allowedNavigations = GetPublishNavigations(model, entry.Metadata.ClrType);

        foreach (var navigation in entry.Metadata.GetNavigations())
        {
            // Only traverse navigations that appear in publish paths
            if (!allowedNavigations.Contains(navigation.Name))
            {
                continue;
            }

            var navigationEntry = entry.Navigation(navigation.Name);

            if (!navigationEntry.IsLoaded)
            {
                navigationEntry.Load();
            }

            var currentValue = navigationEntry.CurrentValue;
            if (currentValue == null)
            {
                continue;
            }

            var childEntry = dbContext.Entry(currentValue);
            HydratePublish(childEntry, visited);
        }
    }

    private static HashSet<string> GetPublishNavigations(EntityEventingModel model, Type clrType)
    {
        if (!model.Metadata.TryGetValue(clrType, out var metadata))
        {
            return [];
        }

        return metadata.Properties
            .Select(p => p.Split('.')[0])
            .ToHashSet();
    }
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

                        var navigationEntry = entityEntry.Navigation(navigation.Name);

                        if (!navigationEntry.IsLoaded)
                        {
                            navigationEntry.Load();
                        }
                    }

                    break;
                }
            case EntityState.Modified:
                {
                    var dbEntry = LoadDbEntry(entityEntry);

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

                        var dbValue = dbEntry.Property(property.Name).CurrentValue;

                        entityEntry.OriginalValues[property.Name] = dbValue;

                        var isStoreGenerated = property.ValueGenerated == ValueGenerated.OnAdd ||
                                               property.ValueGenerated == ValueGenerated.OnAddOrUpdate ||
                                               property.GetAfterSaveBehavior() == PropertySaveBehavior.Ignore;

                        if (isStoreGenerated)
                        {
                            entityEntry.Property(property.Name).CurrentValue = dbValue;
                        }
                    }

                    break;
                }
            case EntityState.Deleted:
                {
                    var dbEntry = LoadDbEntry(entityEntry);

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

                        var dbValue = dbEntry.Property(property.Name).CurrentValue;

                        entityEntry.OriginalValues[property.Name] = dbValue;
                    }

                    break;
                }
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
        var loaderCache = LoaderCache.GetOrAdd(clrType, x =>
        {
            var method = typeof(EntityGraphHydrator)
                .GetMethod(nameof(LoadEntityNoTrackingGeneric), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(x);

            return (Func<DbContext, IEntityType, object[], object?>)method
                .CreateDelegate(typeof(Func<DbContext, IEntityType, object[], object?>));
        });

        return loaderCache(this.dbContext, entityType, keyValues);
    }
    private static object? LoadEntityNoTrackingGeneric<TEntity>(DbContext context, IEntityType entityType, object[] keyValues)
        where TEntity : class
    {
        var predicate = EntityKeyPredicateCache.GetPredicate<TEntity>(entityType, keyValues);

        return context
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefault(predicate);
    }



    internal void HydrateAlongPath(EntityEntry entry, ReversePublishPlan plan)
    {
        var currentEntry = entry;

        foreach (var step in plan.Path)
        {
            var navigationEntry = currentEntry.Navigation(step.NavigationName);

            if (!navigationEntry.IsLoaded)
            {
                navigationEntry.Load();
            }

            var currentValue = navigationEntry.CurrentValue;

            if (currentValue == null)
                return;

            currentEntry = currentEntry.Context.Entry(currentValue);

            // Ensure each level is hydrated for publish
            this.HydrateEntry(currentEntry);
        }
    }



    internal IReadOnlyList<EntityEntry> ResolveFromHydratedGraph(EntityEntry startEntry, ReversePublishPlan plan)
    {
        var currentEntries = new List<EntityEntry> { startEntry };

        foreach (var step in plan.Path.Reverse())
        {
            var nextEntries = new List<EntityEntry>();

            foreach (var entry in currentEntries)
            {
                var fk = step.ForeignKey;

                if (!step.IsOnDependent)
                {
                    if (entry.Metadata != fk.DeclaringEntityType)
                        continue;

                    nextEntries.AddRange(GetRelatedEntries(entry, fk, goToPrincipal: true));
                }
                else
                {
                    if (entry.Metadata != fk.PrincipalEntityType)
                        continue;

                    nextEntries.AddRange(GetRelatedEntries(entry, fk, goToPrincipal: false));
                }
            }

            currentEntries = nextEntries;

            if (currentEntries.Count == 0)
            {
                break;
            }
        }

        return currentEntries;
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

}