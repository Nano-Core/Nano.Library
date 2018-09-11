using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Extensions;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Data.Extensions;
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
        protected virtual TContext Context { get; }

        /// <summary>
        /// Is Lazy Loading Enabled.
        /// </summary>
        public virtual bool IsLazyLoadingEnabled
        {
            get
            {
                return this.Context.ChangeTracker.LazyLoadingEnabled;
            }
            set
            {
                this.Context.ChangeTracker.LazyLoadingEnabled = value;
            }
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual DbSet<TEntity> GetEntitySet<TEntity>()
            where TEntity : class, IEntity
        {
            return this.Context.Set<TEntity>();
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync<TEntity, TIdentity>(TIdentity key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TIdentity>(IEnumerable<TIdentity> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.GetEntitySet<TEntity>()
                .Where(x => keys.Any(y => y.Equals(x)))
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.GetEntitySet<TEntity>()
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, IQueryCriteria, new()
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.GetEntitySet<TEntity>()
                .Where(query.Criteria)
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            return await this.GetEntitySet<TEntity>()
                .Where(where)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, IPagination pagination, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            return await this.GetEntitySet<TEntity>()
                .Where(where)
                .Limit(pagination)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, IPagination pagination, IOrdering ordering, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            return await this.GetEntitySet<TEntity>()
                .Where(where)
                .Limit(pagination)
                .Order(ordering)
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

            await this.GetEntitySet<TEntity>()
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

            await this.GetEntitySet<TEntity>()
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

            await this.GetEntitySet<TEntity>()
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

            await this.GetEntitySet<TEntity>()
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