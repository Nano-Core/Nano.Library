using System;
using System.Linq;
using Nano.App.Controllers.Contracts;
using Nano.App.Models.Interfaces;

namespace Nano.Data.Extensions
{
    /// <summary>
    /// Queryable Extensions.
    /// See <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Gets the <see cref="IQueryable{T}"/> from the passed <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="criteria">The <see cref="IEntity"/>.</param>
        /// <returns>The <see cref="Criteria"/>.</returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> queryable, Criteria criteria)
            where TEntity : IEntity
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (criteria.AfterAt.HasValue)
                queryable = queryable.Where(x => x.CreatedAt >= criteria.AfterAt.Value);

            if (criteria.BeforeAt.HasValue)
                queryable = queryable.Where(x => x.CreatedAt >= criteria.BeforeAt.Value);

            return queryable
                .Where(x => x.IsActive == criteria.IsActive);
        }
    }
}