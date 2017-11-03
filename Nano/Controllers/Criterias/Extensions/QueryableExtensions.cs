using System;
using System.Linq;
using System.Linq.Expressions;
using Nano.Controllers.Criterias.Entities;
using Nano.Controllers.Criterias.Enums;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Criterias.Extensions
{
    /// <summary>
    /// Queryable Extensions.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Adds where clause to the <see cref="IQueryable{T}"/> based on the passed <paramref name="criteria"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type used in the <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="criteria">The <see cref="ICriteria"/>.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, ICriteria criteria)
            where TEntity : class, IEntity
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var filter = criteria.GetExpression<TEntity>();
            var expression = new FilterBuilder().GetExpression<TEntity>(filter);

            return source.Where(expression);
        }

        /// <summary>
        /// Adds order by clause to the <see cref="IQueryable{T}"/> based on the passed <paramref name="ordering"/>
        /// </summary>
        /// <typeparam name="TEntity">The type used in the <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="ordering">The <see cref="Ordering"/>.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<TEntity> Order<TEntity>(this IQueryable<TEntity> source, Ordering ordering = null)
            where TEntity : IEntity
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            ordering = ordering ?? new Ordering();

            var parameter = Expression.Parameter(typeof(TEntity));
            var property = Expression.Property(parameter, ordering.By);

            if (property.Type == typeof(Guid))
            {
                var expression = Expression.Lambda<Func<TEntity, Guid>>(property, parameter);
                return ordering.Direction == SortDirection.Asc ? source.OrderBy(expression) : source.OrderByDescending(expression);
            }
            else
            {
                var expression = Expression.Lambda<Func<TEntity, dynamic>>(property, parameter);
                return ordering.Direction == SortDirection.Asc ? source.OrderBy(expression) : source.OrderByDescending(expression);
            }
        }

        /// <summary>
        /// Adds skip and take clauses to the <see cref="IQueryable{T}"/> based on the passed <paramref name="pagination"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type used in the <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/>.</param>
        /// <param name="pagination">The <see cref="Pagination"/>.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<TEntity> Limit<TEntity>(this IQueryable<TEntity> source, Pagination pagination = null)
            where TEntity : IEntity
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            pagination = pagination ?? new Pagination();

            return source
                .Skip(pagination.Skip)
                .Take(pagination.Count);
        }
    }
}