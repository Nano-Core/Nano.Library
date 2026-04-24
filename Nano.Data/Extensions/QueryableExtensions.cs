using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Config.Enums;

namespace Nano.Data.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to include related entities marked with the <see cref="IncludeAttribute"/>.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Recursively includes all navigation properties of <typeparamref name="T"/> that have the <see cref="IncludeAttribute"/> applied.
    /// </summary>
    /// <typeparam name="T">The type of entity in the queryable.</typeparam>
    /// <param name="queryable">The <see cref="IQueryable{T}"/> to include navigations for.</param>
    /// <param name="maxDepth">The maximum recursion depth for including nested properties.</param>
    /// <returns>An <see cref="IQueryable{T}"/> with all annotated navigations included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="queryable"/> is <c>null</c>.</exception>
    public static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable, int maxDepth)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(queryable);

        return queryable
            .IncludeAnnotations(typeof(T), string.Empty, maxDepth);
    }

    internal static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable, Type type, string name, int depth)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(queryable);
        ArgumentNullException.ThrowIfNull(type);

        if (depth <= 0)
        {
            return queryable;
        }

        var includedPropertyInfos = type
            .GetProperties()
            .Select(x => new
            {
                PropertyInfo = x,
                IncludeAttribute = x.GetCustomAttribute<IncludeAttribute>()
            })
            .Where(x => x.IncludeAttribute != null);

        foreach (var includePropertyInfo in includedPropertyInfos)
        {
            var navigationName = string.IsNullOrEmpty(name)
                ? includePropertyInfo.PropertyInfo.Name
                : $"{name}.{includePropertyInfo.PropertyInfo.Name}";

            var nextType = includePropertyInfo.PropertyInfo.PropertyType.IsGenericType
                ? includePropertyInfo.PropertyInfo.PropertyType.GenericTypeArguments[0]
                : includePropertyInfo.PropertyInfo.PropertyType;

            queryable = queryable
                .Include(navigationName);

            if (includePropertyInfo.IncludeAttribute?.QuerySplitBehavior == QuerySplitBehavior.SplitQuery)
            {
                queryable = queryable
                    .AsSplitQuery();
            }

            queryable = queryable
                .IncludeAnnotations(nextType, navigationName, depth - 1);
        }

        return queryable;
    }
}