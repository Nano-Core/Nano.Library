using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nano.Models.Attributes;

namespace Nano.Data.Extensions
{
    /// <summary>
    /// Queryable Extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Includes all model associations in the query, which has the <see cref="IncludeAttribute"/> defined.
        /// </summary>
        /// <typeparam name="T">The type of the queryable.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="maxDepth">The max include indention.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable, int maxDepth)
            where T : class
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable
                .IncludeAnnotations(typeof(T), string.Empty, maxDepth);
        }

        /// <summary>
        /// Includes all model associations in the query, which has the <see cref="IncludeAttribute"/> defined.
        /// </summary>
        /// <typeparam name="T">The type of the queryable.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="type">The <see cref="Type"/> of entity to include from.</param>
        /// <param name="name">The name of the property navigation.</param>
        /// <param name="depth">The current depth, when including nested navigation properties.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        internal static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable, Type type, string name, int depth)
            where T : class
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (depth <= 0)
                return queryable;

            type
                .GetProperties()
                .Where(x => x.GetCustomAttributes<IncludeAttribute>().Any())
                .ToList()
                .ForEach(x =>
                {
                    var navigationName = string.IsNullOrEmpty(name) 
                        ? x.Name 
                        : $"{name}.{x.Name}";

                    var nextType = x.PropertyType.IsGenericType 
                        ? x.PropertyType.GenericTypeArguments[0] 
                        : x.PropertyType;

                    queryable = queryable
                        .Include(navigationName)
                        .IncludeAnnotations(nextType, navigationName, depth - 1);
                });

            return queryable;
        }
    }
}