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
    private readonly record struct PlanKey(Type RootType, string NavigationPath);

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

            if (publishProperties.Count > 0)
            {
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
            }

            foreach (var path in publishProperties)
            {
                entityEventingModel.Accessors[(clrType, path)] = CompileAccessor(clrType, path);

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

        ValidatePublishProperties(entityType, properties);

        return properties;
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


    private static void BuildReversePlans(
        EntityEventingModel model,
        IEntityType rootEntityType,
        Type rootClrType,
        IEnumerable<string> publishPaths)
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

                    RegisterPlan(
                        model,
                        rootClrType,
                        changedType,
                        navigationSegments,
                        leafProperty);

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
                ForeignKey = n.ForeignKey,
                IsOnDependent = n.IsOnDependent
            }).ToArray(),
            WatchedProperties = new HashSet<string> { leafProperty },
            //ResolveRoots = BuildResolver(rootType, navigationPath)
        };

        plans.Add(plan);
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
    private static Func<DbContext, object, IReadOnlyList<object>> BuildResolver(Type rootType, List<INavigation> path)
    {
        return (db, changedEntity) =>
        {
            return null!;
            //var currentObjects = new List<object> { changedEntity };

            //// walk backwards
            //for (int i = path.Count - 1; i >= 0; i--)
            //{
            //    var nav = path[i];
            //    var fk = nav.ForeignKey;

            //    var next = new List<object>();

            //    foreach (var obj in currentObjects)
            //    {
            //        if (nav.IsOnDependent)
            //        {
            //            // dependent → principal (single)
            //            var principal = GetPrincipal(db, obj, fk);
            //            if (principal != null)
            //                next.Add(principal);
            //        }
            //        else
            //        {
            //            // principal → dependents (many)
            //            var dependents = GetDependents(db, obj, fk);
            //            next.AddRange(dependents);
            //        }
            //    }

            //    currentObjects = next;
            //}

            //return currentObjects;
        };
    }


    //private static object? GetPrincipal(DbContext db, object dependent, IForeignKey fk)
    //{
    //    var keyValues = fk.Properties
    //        .Select(p => p.PropertyInfo!.GetValue(dependent))
    //        .ToArray();

    //    return Find(db, fk.PrincipalEntityType.ClrType, fk.PrincipalKey.Properties, keyValues);
    //}

    //private static IEnumerable<object> GetDependents(DbContext db, object principal, IForeignKey fk)
    //{
    //    var keyValues = fk.PrincipalKey.Properties
    //        .Select(p => p.PropertyInfo!.GetValue(principal))
    //        .ToArray();

    //    return Where(db, fk.DeclaringEntityType.ClrType, fk.Properties, keyValues);
    //}
    //private static object? Find(
    //    DbContext db,
    //    Type type,
    //    IReadOnlyList<IProperty> keyProps,
    //    object[] values)
    //{
    //    var set = db.Set(type);

    //    return set.Find(values);
    //}

    //private static IEnumerable<object> Where(
    //    DbContext db,
    //    Type type,
    //    IReadOnlyList<IProperty> props,
    //    object[] values)
    //{
    //    var set = db.Set(type).AsQueryable();

    //    foreach (var entity in set)
    //    {
    //        bool match = true;

    //        for (int i = 0; i < props.Count; i++)
    //        {
    //            var val = props[i].PropertyInfo!.GetValue(entity);
    //            if (!Equals(val, values[i]))
    //            {
    //                match = false;
    //                break;
    //            }
    //        }

    //        if (match)
    //            yield return entity;
    //    }
    //}
}