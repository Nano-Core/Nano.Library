using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Extensions;
using Nano.Data.Eventing.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            var publishProperties = GetPublishProperties(entityType, clrType);

            if (clrType.IsTypeOf(typeof(BaseEntityReadOnly<>)))
            {
                publishProperties
                    .Add(nameof(BaseEntity<>.CreatedAt));
            }

            entityEventingModel.Metadata[clrType] = new PublishMetadata
            {
                Properties = publishProperties
                    .ToArray()
            };

            foreach (var property in publishProperties)
            {
                entityEventingModel.Accessors[(clrType, property)] = CompileAccessor(clrType, property);

                BuildReversePlans(entityEventingModel, entityType, clrType, publishProperties);
            }
        }

        return entityEventingModel;
    }
    private static HashSet<string> GetPublishProperties(IEntityType entityType, Type clrType)
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(clrType);

        var properties = new HashSet<string>();

        var current = clrType;

        while (current != typeof(object) && current.IsTypeOf(typeof(IEntityIdentity<>)))
        {
            var publishAttribute = current
                .GetCustomAttribute<PublishAttribute>(false);

            if (publishAttribute != null)
            {
                foreach (var name in publishAttribute.PropertyNames)
                {
                    properties
                        .Add(name);
                }
            }

            current = current.BaseType!;
        }

        return properties;
    }
    private static Func<object, object?> CompileAccessor(Type rootType, string path)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(path);

        var parameter = Expression.Parameter(typeof(object), "obj");
        Expression current = Expression.Convert(parameter, rootType);

        foreach (var part in path.Split('.'))
        {
            var property = current.Type.GetProperty(part) ?? throw new InvalidOperationException($"Property '{part}' not found on '{current.Type.Name}'");

            var equalExpression = Expression.Equal(current, Expression.Constant(null));
            var trueExpression = Expression.Default(property.PropertyType);
            var falsexpression = Expression.Property(current, property);

            current = Expression.Condition(equalExpression, trueExpression, falsexpression);
        }

        var lambda = Expression.Lambda<Func<object, object?>>(Expression.Convert(current, typeof(object)), parameter);
            
        return lambda
            .Compile();
    }






    private static void BuildReversePlans(EntityEventingModel model, IEntityType rootEntityType, Type rootClrType, IEnumerable<string> publishPaths)
    {
        foreach (var path in publishPaths)
        {
            var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

            var current = rootEntityType;
            var navigationSegments = new List<INavigation>();

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                var navigation = current.FindNavigation(segment);
                if (navigation == null)
                {
                    // reached scalar → previous entity is the ChangedType
                    var changedType = current.ClrType;
                    var leafProperty = segment;

                    RegisterPlan(model, rootClrType, changedType, navigationSegments, leafProperty);

                    break;
                }

                navigationSegments.Add(navigation);
                current = navigation.TargetEntityType;
            }
        }
    }
    private static void RegisterPlan(EntityEventingModel model, Type rootType, Type changedType, List<INavigation> navigationPath, string leafProperty)
    {
        if (!model.ReversePlans.TryGetValue(changedType, out var plans))
        {
            plans = [];
            model.ReversePlans[changedType] = plans;
        }

        var existing = plans.FirstOrDefault(p =>
            p.RootType == rootType &&
            PathEquals(p.Path, navigationPath));

        if (existing != null)
        {
            existing.WatchedProperties.Add(leafProperty);
            return;
        }

        var plan = new ReversePublishPlan
        {
            RootType = rootType,
            ChangedType = changedType,
            Path = navigationPath.Select(n => new NavigationStep
            {
                NavigationName = n.Name,
                TargetType = n.ClrType,
                ForeignKey = n.ForeignKey,
                IsOnDependent = n.IsOnDependent
            }).ToArray(),
            WatchedProperties = new HashSet<string> { leafProperty }
        };

        plans.Add(plan);


        if (plan.Path.Count == 0)
            return;

        if (!model.TraversalGraphs.TryGetValue(plan.RootType, out var graph))
        {
            graph = new TraversalGraph();
            model.TraversalGraphs[plan.RootType] = graph;
        }

        var currentType = plan.RootType;

        foreach (var step in plan.Path)
        {
            graph.AddEdge(currentType, step.NavigationName);
            currentType = step.TargetType;
        }
    }
    private static bool PathEquals(IReadOnlyList<NavigationStep> a, IReadOnlyList<INavigation> b)
    {
        if (a.Count != b.Count)
            return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].NavigationName != b[i].Name)
                return false;
        }

        return true;
    }








    private static void ValidatePublishProperties(IEntityType rootEntityType, IEnumerable<string> paths)
    {
        ArgumentNullException.ThrowIfNull(rootEntityType);
        ArgumentNullException.ThrowIfNull(paths);

        // BUG: We should validate that no paths ends with the same name. the contract is flatten when publishing, so it would create duplicates

        foreach (var path in paths)
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

                current = navigation.TargetEntityType;
            }
        }
    }




    //private static void AddToTraversalGraph(EntityEventingModel model, ReversePublishPlan plan)
    //{
    //    if (plan.Path.Count == 0)
    //        return;

    //    if (!model.TraversalGraphs.TryGetValue(plan.RootType, out var graph))
    //    {
    //        graph = new TraversalGraph();
    //        model.TraversalGraphs[plan.RootType] = graph;
    //    }

    //    var currentType = plan.RootType;

    //    foreach (var step in plan.Path)
    //    {
    //        graph.AddEdge(currentType, step.NavigationName);
    //        currentType = step.TargetType;
    //    }
    //}

    //private static Dictionary<Type, TraversalGraph> BuildTraversalGraph(EntityEventingModel model)
    //{
    //    var result = new Dictionary<Type, TraversalGraph>();

    //    foreach (var kvp in model.ReversePlans)
    //    {
    //        var changedType = kvp.Key;
    //        var plans = kvp.Value;

    //        foreach (var plan in plans)
    //        {
    //            if (!result.TryGetValue(plan.RootType, out var graph))
    //            {
    //                graph = new TraversalGraph();
    //                result[plan.RootType] = graph;
    //            }

    //            BuildFromPlan(graph, plan);
    //        }
    //    }

    //    return result;
    //}
    //private static void BuildFromPlan(TraversalGraph graph, ReversePublishPlan plan)
    //{
    //    if (plan.Path.Count == 0)
    //    {
    //        return;
    //    }

    //    var currentType = plan.RootType;

    //    foreach (var step in plan.Path)
    //    {
    //        // Register edge: currentType -> navigation
    //        graph.AddEdge(currentType, step.NavigationName);

    //        // Move to next type
    //        currentType = ResolveNextType(currentType, step);
    //    }
    //}
    //private static Type ResolveNextType(Type currentType, NavigationStep step)
    //{
    //    // IMPORTANT:
    //    // You likely already have FK metadata available when building plans.
    //    // If NavigationStep doesn't include target type, you MUST add it.

    //    if (step.TargetType is null)
    //    {
    //        throw new InvalidOperationException(
    //            $"NavigationStep for '{step.NavigationName}' is missing TargetType.");
    //    }

    //    return step.TargetType;
    //}
}