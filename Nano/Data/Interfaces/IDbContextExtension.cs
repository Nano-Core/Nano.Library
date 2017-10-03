using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data.Interfaces
{
    /// <summary>
    /// Extensions to <see cref="IDbContext"/>.
    /// </summary>
    public interface IDbContextExtension
    {
        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task<EntityEntry<TEntity>> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// Updates a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task UpdateRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task<EntityEntry<TEntity>> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// Removes a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        Task RemoveRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);
    }
}