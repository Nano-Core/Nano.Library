using System;
using System.Linq;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Contracts.Extensions
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
        /// <param name="criteria">The <see cref="ICriteria"/>.</param>
        /// <returns>he <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<TEntity> Query<TEntity>(this IQueryable<TEntity> queryable, ICriteria criteria)
            where TEntity : IEntity
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return criteria
                .GetQuery(queryable);
        }
    }
}