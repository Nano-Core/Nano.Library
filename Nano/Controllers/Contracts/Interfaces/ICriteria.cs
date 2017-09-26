using System;
using System.Linq;
using Nano.Models.Interfaces;

namespace Nano.Controllers.Contracts.Interfaces
{
    /// <summary>
    /// Criteria interface.
    /// </summary>
    public interface ICriteria
    {
        /// <summary>
        /// Is Active (read-only).
        /// Only entities where <see cref="IEntity.IsActive"/> is 'true' can be found.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// The <see cref="IEntity.CreatedAt"/> must be equal or greater than.
        /// </summary>
        DateTimeOffset? AfterAt { get; set; }

        /// <summary>
        /// The <see cref="IEntity.CreatedAt"/> must be equal or less than.
        /// </summary>
        DateTimeOffset? BeforeAt { get; set; }

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> of the <see cref="ICriteria"/> implementation.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{TEntity}"/>.</param>
        /// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
        IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> queryable)
            where TEntity : IEntity;
    }
}