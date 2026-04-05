using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nano.Data;

internal static class ForeignKeyQueryCache
{
    private static readonly ConcurrentDictionary<ForeignKeyMap, Func<DbContext, object[], IQueryable>> cache = new();

    internal static Func<DbContext, object[], IQueryable> Get(Type rootType, IReadOnlyList<IProperty> properties)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(properties);

        var propertyNames = properties
            .Select(x => x.Name)
            .ToArray();

        var foreignKeyMap = new ForeignKeyMap(rootType, propertyNames);

        return cache
            .GetOrAdd(foreignKeyMap, _ => BuildFactory(rootType, properties));
    }


    private static Func<DbContext, object[], IQueryable> BuildFactory(Type rootType, IReadOnlyList<IProperty> properties)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(properties);

        var accessor = DbSetAccessorCache.Get(rootType);

        var entityParameter = Expression.Parameter(rootType, "e");
        var contextParameter = Expression.Parameter(typeof(DbContext), "ctx");
        var valuesParameter = Expression.Parameter(typeof(object[]), "values");
        
        var invoke = Expression.Invoke(Expression.Constant(accessor), contextParameter);

        Expression? predicate = null;

        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertyExpression = Expression.Property(entityParameter, property.Name);
            var valueExpression = Expression.ArrayIndex(valuesParameter, Expression.Constant(i));
            var convertedValueExpression = Expression.Convert(valueExpression, propertyExpression.Type);
            var equalExpression = Expression.Equal(propertyExpression, convertedValueExpression);

            predicate = predicate == null
                ? equalExpression
                : Expression.AndAlso(predicate, equalExpression);
        }

        var lambdaExpression = Expression.Lambda(predicate!, entityParameter);

        var whereMethod = typeof(Queryable)
            .GetMethods()
            .First(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2)
            .MakeGenericMethod(rootType);

        var whereCall = Expression.Call(whereMethod, invoke, lambdaExpression);

        return Expression.Lambda<Func<DbContext, object[], IQueryable>>(whereCall, contextParameter, valuesParameter
        ).Compile();
    }
}