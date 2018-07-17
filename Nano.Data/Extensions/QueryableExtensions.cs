using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Attributes;

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
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable)
            where T : class
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            var type = typeof(T);
            var properties = type.GetProperties();

            properties
                .Where(x => x.GetCustomAttributes<IncludeAttribute>().Any())
                .ToList()
                .ForEach(x =>
                {
                    queryable
                        .Include(x.Name);
                });

            return queryable;
        }
    }
}