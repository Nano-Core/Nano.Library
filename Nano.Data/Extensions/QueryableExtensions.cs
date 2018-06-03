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
        /// <param name="queryable"></param>
        /// <returns></returns>
        public static IQueryable<T> IncludeAnnotations<T>(this IQueryable<T> queryable)
            where T : class
        {
            var includes = typeof(T)
                .GetProperties()
                .Where(x => x.GetCustomAttributes<IncludeAttribute>().Any())
                .Aggregate(string.Empty, (current, include) => $"{current}{include.Name};");

            if (!string.IsNullOrEmpty(includes))
            {
                queryable
                    .Include(includes);
            }

            return queryable;
        }
    }
}