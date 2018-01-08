using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data
{
    /// <inheritdoc />
    public abstract class BaseDbContext : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="DbContextOptions"/>.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {

        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        public virtual async Task<EntityEntry<TEntity>> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await Task.Factory
                .StartNew(() =>
                {
                    var entry = this.Update(entity);

                    return entry;
                }, cancellationToken);
        }

        /// <summary>
        /// Updates a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        public virtual async Task UpdateRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                await this.UpdateAsync(entity, cancellationToken);
            }
        }

        /// <summary>
        /// Removes the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        public virtual async Task<EntityEntry<TEntity>> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await Task.Factory
                .StartNew(() =>
                {
                    var entry = this.Remove(entity);

                    return entry;
                }, cancellationToken);
        }

        /// <summary>
        /// Removes a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        public virtual async Task RemoveRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                await this.RemoveAsync(entity, cancellationToken);
            }
        }

        /// <summary>
        /// Adds or updates (if exists) the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <returns>A <see cref="EntityEntry{TEntity}"/>.</returns>
        public virtual EntityEntry<TEntity> AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var dbSet = this.Set<TEntity>();
            var tracked = dbSet.SingleOrDefault(x => x == entity);

            if (tracked != null)
            {
                this.Entry(tracked).CurrentValues.SetValues(entity);
                return this.Entry(tracked);
            }

            return dbSet.Add(entity);
        }

        /// <summary>
        /// Adds or updates (if exists) a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        public virtual void AddOrUpdateMany(IEnumerable<object> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                this.AddOrUpdate(entity);
            }
        }

        /// <summary>
        /// Adds or updates (if exists) the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
        /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task<EntityEntry<TEntity>> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await Task.Factory
                .StartNew(() =>
                {
                    var entry = this.AddOrUpdate(entity);

                    return entry;
                }, cancellationToken);
        }

        /// <summary>
        /// Adds or updates (if exists) a range of entities.
        /// </summary>
        /// <param name="entities">The <see cref="object"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        public virtual async Task AddOrUpdateManyAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await Task.Factory
                .StartNew(() =>
                {
                    this.AddOrUpdateMany(entities);
                }, cancellationToken);
        }
    }
}