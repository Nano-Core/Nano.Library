using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Data.Interfaces;

namespace Nano.Data
{
    /// <inheritdoc cref="IDbContext"/>
    public abstract class BaseDbContext : DbContext, IDbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.EntityFrameworkCore.DbContextOptions"/>.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        /// <inheritdoc />
        public Task<EntityEntry<TEntity>> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) 
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Update(entity), cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() =>
                {
                    foreach (var entity in entities)
                    {
                        this.Update(entity);
                    }
                }, cancellationToken);
        }

        /// <inheritdoc />
        public Task<EntityEntry<TEntity>> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Remove(entity), cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() =>
                {
                    foreach (var entity in entities)
                    {
                        this.Remove(entity);
                    }
                }, cancellationToken);
        }
    }
}