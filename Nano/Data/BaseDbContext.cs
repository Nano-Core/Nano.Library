using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="options">The <see cref="DbContextOptions"/>.</param>
        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        /// <inheritdoc />
        public virtual TEntity GetOrAdd<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var tracked = this.Set<TEntity>().SingleOrDefault(x => x == entity);
            if (tracked != null)
            {
                return tracked;
            }

            this.Set<TEntity>().Add(entity);

            return entity;
        }

        /// <inheritdoc />
        public virtual Task<EntityEntry<TEntity>> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) 
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Update(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task UpdateRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
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
        public virtual Task<EntityEntry<TEntity>> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.Remove(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task RemoveRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual Task<EntityEntry<TEntity>> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Task.Factory
                .StartNew(() => this.AddOrUpdate(entity), cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task AddOrUpdateManyAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            return Task.Factory
                .StartNew(() =>
                {
                    foreach (var entity in entities)
                    {
                        this.AddOrUpdate(entity);
                    }
                }, cancellationToken);
        }
    }
}