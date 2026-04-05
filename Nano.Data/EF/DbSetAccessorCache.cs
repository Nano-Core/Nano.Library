using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Nano.Data;

internal static class DbSetAccessorCache
{
    private static readonly ConcurrentDictionary<Type, LambdaExpression> cache = new();

    public static Expression Get(Type entityType)
    {
        ArgumentNullException.ThrowIfNull(entityType);

        return cache
            .GetOrAdd(entityType, x =>
            {
                var method = typeof(DbContext)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Single(y => y is { Name: nameof(DbContext.Set), IsGenericMethodDefinition: true } && y.GetParameters().Length == 0)
                    .MakeGenericMethod(x);

                var contextParameter = Expression.Parameter(typeof(DbContext), "ctx");
                var body = Expression.Call(contextParameter, method);

                return Expression.Lambda(body, contextParameter);
            });
    }
}