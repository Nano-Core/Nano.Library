using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Eventing.TypeMap;

internal static class EntityEventingTypeMapCache
{
    private static readonly ConcurrentDictionary<Type, Dictionary<Type, EntityEventingMetadata>> cache = new();

    public static Dictionary<Type, EntityEventingMetadata> GetOrCreate(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var contextType = dbContext.GetType();

        return cache
            .GetOrAdd(contextType, _ => BuildFactory(dbContext));
    }


    private static Dictionary<Type, EntityEventingMetadata> BuildFactory(DbContext dbContext)
    {
        var entityTypes = dbContext.Model.GetEntityTypes().ToList();

        var entityTypeLookup = entityTypes.ToDictionary(x => x.ClrType);

        var publishableTypes = entityTypes.Select(x => x.ClrType)
            .Where(t => t.IsTypeOf(typeof(IEntityIdentity<>)))
            .ToHashSet();

        var result = new Dictionary<Type, EntityEventingMetadata>();

        foreach (var entityType in entityTypes)
        {
            var clrType = entityType.ClrType;

            if (!publishableTypes.Contains(clrType))
            {
                continue;
            }

            var publishHierarchy = GetPublishHierarchyTypes(clrType).ToArray();

            if (publishHierarchy.Length == 0)
            {
                continue;
            }

            var properties = GetAggregatedPublishProperties(entityType, clrType);

            if (properties.Length == 0)
            {
                continue;
            }

            result[clrType] = BuildMetadata(entityType, clrType, properties);

            foreach (var baseType in publishHierarchy)
            {
                if (baseType.IsAbstract || !publishableTypes.Contains(baseType) || result.ContainsKey(baseType))
                {
                    continue;
                }

                if (!entityTypeLookup.TryGetValue(baseType, out var baseEntityType))
                {
                    continue;
                }

                var baseProps = GetAggregatedPublishProperties(baseEntityType, baseType);

                if (baseProps.Length == 0)
                {
                    continue;
                }

                result[baseType] = BuildMetadata(baseEntityType, baseType, baseProps);
            }
        }

        return result;
    }
    private static EntityEventingMetadata BuildMetadata(IEntityType entityType, Type clrType, string[] properties)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(clrType);
        ArgumentNullException.ThrowIfNull(properties);

        var navigationMap = entityType
            .GetNavigations()
            .ToDictionary(x => x.Name, n => n.TargetEntityType.ClrType);

        var reverseDependencies = new Dictionary<Type, List<string>>();

        foreach (var prop in properties)
        {
            var segments = prop
                .Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                continue;
            }

            var current = entityType;
            Type? targetType = null;

            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                var isLast = i == segments.Length - 1;

                if (isLast)
                {
                    continue;
                }

                var navigation = current
                    .FindNavigation(segment);

                if (navigation == null)
                {
                    break;
                }

                targetType = navigation.TargetEntityType.ClrType;
                current = navigation.TargetEntityType;
            }

            if (targetType == null)
            {
                continue;
            }

            if (!reverseDependencies.TryGetValue(targetType, out var reverseDependency))
            {
                reverseDependency = [];
                reverseDependencies[targetType] = reverseDependency;
            }

            reverseDependency
                .Add(prop);
        }

        return new EntityEventingMetadata
        {
            ClrType = clrType,
            Properties = properties,
            PropertySegments = properties
                .Select(x => x.Split('.'))
                .ToArray(),
            Navigations = navigationMap,
            ReverseDependencies = reverseDependencies
        };
    }
    private static IEnumerable<Type> GetPublishHierarchyTypes(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var current = type;

        while (current != typeof(object))
        {
            if (Attribute.IsDefined(current, typeof(PublishAttribute), false))
            {
                yield return current;
            }

            current = current.BaseType!;
        }
    }
    private static string[] GetAggregatedPublishProperties(IEntityType entityType, Type clrType)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(clrType);

        var properties = new HashSet<string>();
        var current = clrType;

        while (current != typeof(object) && typeof(IEntity).IsAssignableFrom(current))
        {
            var attr = current
                .GetCustomAttribute<PublishAttribute>(false);

            if (attr != null)
            {
                foreach (var name in attr.PropertyNames)
                {
                    properties
                        .Add(name);
                }
            }

            current = current.BaseType!;
        }

        properties
            .Add(nameof(BaseEntity<>.CreatedAt));

        var result = properties.ToArray();

        ValidatePublishProperties(entityType, result);

        return result;
    }
    private static void ValidatePublishProperties(IEntityType rootEntityType, IEnumerable<string> properties)
    {
        ArgumentNullException.ThrowIfNull(rootEntityType);
        ArgumentNullException.ThrowIfNull(properties);

        foreach (var path in properties)
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
                    if (current.FindProperty(segment) == null)
                    {
                        throw new InvalidOperationException($"Property '{segment}' not found on '{current.Name}'");
                    }

                    break;
                }

                var navigation = current
                    .FindNavigation(segment);

                if (navigation == null)
                {
                    throw new InvalidOperationException($"Navigation '{segment}' not found on '{current.Name}'");
                }

                current = navigation.TargetEntityType;
            }
        }
    }
}