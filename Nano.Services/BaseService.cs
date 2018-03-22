using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Extensions;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;
using Z.EntityFramework.Plus;

namespace Nano.Services
{
    /// <inheritdoc />
    public abstract class BaseService<TContext> : IService
        where TContext : BaseDbContext
    {
        /// <summary>
        /// Context.
        /// </summary>
        public virtual TContext Context { get; } // BUG: Should be protected.

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        protected BaseService(TContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this.Context = context;
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync<TEntity, TIdentity>(TIdentity key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return await this.Context
                .Set<TEntity>()
                .FindAsync(new[] { key }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TIdentity>(IEnumerable<TIdentity> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.Context
                .Set<TEntity>()
                .Where(x => keys.Any(y => y.Equals(x)))
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, IQueryCriteria, new()
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await this.Context
                .Set<TEntity>()
                .Where(expression)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Query query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = this.Context
                .Add(entity);

            await this.Context
                .SaveChangesAsync(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .AddRangeAsync(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = this.Context
                .Update(entity);

            await this.Context
                .SaveChangesAsync(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            this.Context
                .UpdateRange(entities);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task UpdateManyAsync<TEntity, TCriteria>(TCriteria select, TEntity update, CancellationToken cancellationToken = default) 
            where TEntity : class, IEntityUpdatable 
            where TCriteria : class, IQueryCriteria, new()
        {
            if (select == null)
                throw new ArgumentNullException(nameof(select));

            if (update == null)
                throw new ArgumentNullException(nameof(update));

            await this.Context
                .Set<TEntity>()
                .Where(select)
                .UpdateAsync(x => update, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task UpdateManyAsync<TEntity>(Expression<Func<TEntity, bool>> select, Expression<Func<TEntity, TEntity>> update, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (select == null)
                throw new ArgumentNullException(nameof(select));

            if (update == null)
                throw new ArgumentNullException(nameof(update));

            await this.Context
                .Set<TEntity>()
                .Where(select)
                .UpdateAsync(update, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatableAndUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = this.Context
                .AddOrUpdate(entity);

            await this.Context
                .SaveChangesAsync(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatableAndUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            this.Context
                .AddOrUpdateMany(entities);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
    
            this.Context
                .Remove(entity);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            this.Context
                .RemoveRange(entities, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity, TCriteria>(TCriteria critiera, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (critiera == null)
                throw new ArgumentNullException(nameof(critiera));

            await this.Context
                .Set<TEntity>()
                .Where(critiera)
                .DeleteAsync(x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> select, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (select == null)
                throw new ArgumentNullException(nameof(select));

            await this.Context
                .Set<TEntity>()
                .Where(select)
                .DeleteAsync(x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);
            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Dispose (non-virtual).
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool).
        /// Override in derived classes as needed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            this.Context?.Dispose();
        }
    }
}