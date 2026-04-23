using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nano.Data;

internal static class EntityKeyPredicateCache
{
    private static readonly ConcurrentDictionary<Type, Func<IEntityType, object[], LambdaExpression>> cache = new();

    internal static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(IEntityType entityType, object[] keyValues)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entityType);
        ArgumentNullException.ThrowIfNull(keyValues);

        var factory = cache
            .GetOrAdd(typeof(TEntity), _ => BuildFactory<TEntity>());

        return (Expression<Func<TEntity, bool>>)factory(entityType, keyValues);
    }


    private static Func<IEntityType, object[], LambdaExpression> BuildFactory<TEntity>()
        where TEntity : class
    {
        var clrType = typeof(TEntity);
        var parameter = Expression.Parameter(clrType, "x");

        return (entityType, keyValues) =>
        {
            var primaryKey = entityType
                .FindPrimaryKey();

            if (primaryKey == null)
            {
                throw new NullReferenceException(nameof(primaryKey));
            }

            var keyProperties = primaryKey.Properties;

            Expression? predicate = null;

            for (var i = 0; i < keyProperties.Count; i++)
            {
                var property = keyProperties[i];
                var propertyType = property.ClrType;

                var efPropertyMethod = typeof(EF)
                    .GetMethod(nameof(EF.Property))?
                    .MakeGenericMethod(propertyType);

                if (efPropertyMethod == null)
                {
                    throw new NullReferenceException(nameof(efPropertyMethod));
                }

                var left = Expression.Call(efPropertyMethod, parameter, Expression.Constant(property.Name));
                var right = Expression.Constant(keyValues[i], propertyType);
                var equal = Expression.Equal(left, right);

                predicate = predicate == null
                    ? equal
                    : Expression.AndAlso(predicate, equal);
            }

            return predicate == null
                ? throw new NullReferenceException(nameof(predicate))
                : Expression.Lambda<Func<TEntity, bool>>(predicate, parameter);
        };
    }
}