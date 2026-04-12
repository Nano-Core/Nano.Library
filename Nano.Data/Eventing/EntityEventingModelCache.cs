using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.Data.Eventing.Extensions;

namespace Nano.Data.Eventing;

internal static class EntityEventingModelCache
{
    private static readonly ConcurrentDictionary<Type, EntityEventingModel> cache = new();

    internal static EntityEventingModel GetOrCreate(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        return cache
            .GetOrAdd(dbContext.GetType(), _ => BuildFactory(dbContext));
    }


    private static EntityEventingModel BuildFactory(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityEventingModel = new EntityEventingModel();

        var entityTypes = dbContext.Model
            .GetEntityTypes()
            .Where(x => x.ClrType
                .IsTypeOf(typeof(IEntityIdentity<>)))
            .ToArray();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;
            var publishPaths = GetPublishProperties(entityType);

            if (publishPaths == null)
            {
                continue;
            }

            entityEventingModel.Accessors
                .AddRange(BuildAccessors(clrType, publishPaths));
            
            entityEventingModel.ReversePlans
                .AddRange(BuildReversePublishPlans(entityType, clrType, publishPaths));

            entityEventingModel.Navigations
                .AddRange(BuildNavigationCache(entityEventingModel.ReversePlans));
        }

        return entityEventingModel;
    }
    private static PropertyAccessors BuildAccessors(Type rootClrType, IEnumerable<string> publishPaths)
    {
        ArgumentNullException.ThrowIfNull(rootClrType);
        ArgumentNullException.ThrowIfNull(publishPaths);

        var propertyAccessors = new PropertyAccessors();

        foreach (var path in publishPaths)
        {
            propertyAccessors
                .Add(rootClrType, path);
        }

        return propertyAccessors;
    }
    private static Navigations BuildNavigationCache(ReversePublishPlans reversePublishPlans)
    {
        ArgumentNullException.ThrowIfNull(reversePublishPlans);

        var navigations = new Navigations();

        foreach (var planGroup in reversePublishPlans.Values)
        {
            foreach (var plan in planGroup)
            {
                var currentType = plan.RootType;

                foreach (var step in plan.NavigationSteps)
                {
                    navigations
                        .AddNavigation(plan.RootType, currentType, step.NavigationName);

                    currentType = step.TargetType;
                }
            }
        }

        return navigations;
    }
    private static ReversePublishPlans BuildReversePublishPlans(IEntityType rootEntityType, Type rootClrType, IEnumerable<string> publishPaths)
    {
        ArgumentNullException.ThrowIfNull(rootClrType);
        ArgumentNullException.ThrowIfNull(rootEntityType);
        ArgumentNullException.ThrowIfNull(publishPaths);

        var reversePublishPlans = new ReversePublishPlans();

        foreach (var path in publishPaths)
        {
            var segments = path
                .Split('.', StringSplitOptions.RemoveEmptyEntries);

            var current = rootEntityType;
            var navigationSegments = new List<INavigation>();

            foreach (var segment in segments)
            {
                var navigation = current
                    .FindNavigation(segment);

                if (navigation == null)
                {
                    var changedClrType = current.ClrType;

                    if (reversePublishPlans.TryAddWatchedProperty(rootClrType, changedClrType, navigationSegments, segment))
                    {
                        break;
                    }

                    var publishPlans = reversePublishPlans
                        .GetOrCreatePlans(changedClrType);

                    var navigationSteps = reversePublishPlans
                        .GetNavigationSteps(changedClrType, navigationSegments);

                    publishPlans
                        .Add(new ReversePlan
                        {
                            RootType = rootClrType,
                            ChangedType = changedClrType,
                            NavigationSteps = navigationSteps,
                            WatchedProperties = [segment]
                        });

                    break;
                }

                navigationSegments
                    .Add(navigation);
                
                current = navigation.TargetEntityType;
            }
        }

        return reversePublishPlans;
    }
    
    private static HashSet<string>? GetPublishProperties(IEntityType entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        var current = entityType.ClrType;

        var hasPublishAttribute = false;
        var paths = new HashSet<string>();

        while (current != typeof(object) && current.IsTypeOf(typeof(IEntityIdentity<>)))
        {
            var publishAttribute = current
                .GetCustomAttribute<PublishAttribute>(false);

            if (publishAttribute != null)
            {
                hasPublishAttribute = true;

                foreach (var name in publishAttribute.Paths)
                {
                    paths
                        .Add(name);
                }
            }

            current = current.BaseType!;
        }

        if (!hasPublishAttribute)
        {
            return null;
        }

        if (entityType.ClrType.IsTypeOf(typeof(BaseEntityReadOnly<>)))
        {
            paths
                .Add(nameof(BaseEntity<>.CreatedAt));
        }

        ValidatePublishPathUniqueness(paths);
        ValidatePublishPathsResolvability(entityType, paths);

        return paths;
    }
    private static void ValidatePublishPathsResolvability(IEntityType rootEntityType, IEnumerable<string> publishProperties)
    {
        ArgumentNullException.ThrowIfNull(rootEntityType);
        ArgumentNullException.ThrowIfNull(publishProperties);

        foreach (var path in publishProperties)
        {
            var segments = path
                .Split('.', StringSplitOptions.RemoveEmptyEntries);

            var current = rootEntityType;

            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var isLast = i == segments.Length - 1;

                if (isLast)
                {
                    var property = current
                        .FindProperty(segment);

                    if (property == null)
                    {
                        throw new InvalidOperationException($"Property '{segment}' not found on '{current.Name}'");
                    }

                    break;
                }

                var navigation = current
                    .FindNavigation(segment) ?? throw new InvalidOperationException($"Navigation '{segment}' not found on '{current.Name}'");

                if (navigation.IsCollection)
                {
                    throw new InvalidOperationException($"Collection navigation '{segment}' is not supported.");
                }

                current = navigation.TargetEntityType;
            }
        }
    }
    private static void ValidatePublishPathUniqueness(IEnumerable<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        var duplicates = paths
            .Select(x => x.Split('.', StringSplitOptions.RemoveEmptyEntries).Last())
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException($"Duplicate leaf property names detected: {string.Join(", ", duplicates)}. " + "Flattened publish contract would overwrite values.");
        }
    }
}