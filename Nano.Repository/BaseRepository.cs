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
using Nano.Repository.Interfaces;
using Z.EntityFramework.Plus;

namespace Nano.Repository
{
    /// <inheritdoc />
    public abstract class BaseRepository<TContext> : IRepository
        where TContext : BaseDbContext
    {
        /// <summary>
        /// Context.
        /// </summary>
        internal virtual TContext Context { get; }

        /// <inheritdoc />
        public virtual bool IsLazyLoadingEnabled
        {
            get => this.Context.ChangeTracker.LazyLoadingEnabled;
            set
            {
                this.Context.ChangeTracker.LazyLoadingEnabled = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/>.</param>
        protected BaseRepository(TContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
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
        public virtual async Task<TEntity> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<int>
        {
            return await this
                .GetAsync<TEntity, int>(key, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<long>
        {
            return await this
                .GetAsync<TEntity, long>(key, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<string>
        {
            return await this
                .GetAsync<TEntity, string>(key, cancellationToken);
        }
        
        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<Guid>
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return await this
                .GetAsync<TEntity, Guid>(key, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(where)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, IQueryCriteria, new()
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(query.Criteria)
                .Order(query.Order)
                .Limit(query.Paging)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TIdentity>(IEnumerable<TIdentity> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TIdentity>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(x => keys.Any(y => y.Equals(x.Id)))
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
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

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
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

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
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

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
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

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
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
        public virtual async Task UpdateManyAsync<TEntity, TCriteria>(TCriteria criteria, TEntity update, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (update == null)
                throw new ArgumentNullException(nameof(update));

            await this.GetEntitySet<TEntity>()
                .Where(criteria)
                .UpdateAsync(x => update, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task UpdateManyAsync<TEntity, TCriteria>(Expression<Func<TEntity, bool>> expression, TEntity update, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (update == null)
                throw new ArgumentNullException(nameof(update));

            await this.GetEntitySet<TEntity>()
                .Where(expression)
                .UpdateAsync(x => update, cancellationToken);

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
        public virtual async Task DeleteManyAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            await this.GetEntitySet<TEntity>()
                .Where(criteria)
                .DeleteAsync(x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            await this.GetEntitySet<TEntity>()
                .Where(expression)
                .DeleteAsync(x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);

            await this.Context
                .SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<long> CountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TCriteria : class, IQueryCriteria, new()
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.GetEntitySet<TEntity>()
                .Where(criteria)
                .LongCountAsync(cancellationToken);
        }
        
        /// <inheritdoc />
        public virtual async Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return await this.GetEntitySet<TEntity>()
                .LongCountAsync(expression, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<decimal> SumAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> sumExpr, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (whereExpr == null)
                throw new ArgumentNullException(nameof(whereExpr));
            
            if (sumExpr == null) 
                throw new ArgumentNullException(nameof(sumExpr));

            return await this.GetEntitySet<TEntity>()
                .Where(whereExpr)
                .SumAsync(sumExpr, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<decimal> AverageAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> avgExpr, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (whereExpr == null)
                throw new ArgumentNullException(nameof(whereExpr));

            if (avgExpr == null) 
                throw new ArgumentNullException(nameof(avgExpr));

            return await this.GetEntitySet<TEntity>()
                .Where(whereExpr)
                .AverageAsync(avgExpr, cancellationToken);
        }

        /// <inheritdoc />
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