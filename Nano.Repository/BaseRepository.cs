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
    public abstract class BaseRepository<TContext, TIdentity> : IRepository
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Context.
        /// </summary>
        internal virtual TContext Context { get; }

        /// <inheritdoc />
        public virtual bool IsLazyLoadingEnabled
        {
            get => this.Context.ChangeTracker.LazyLoadingEnabled;
            set => this.Context.ChangeTracker.LazyLoadingEnabled = value;
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
        public virtual async Task<TEntity> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TKey>
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
            return await this
                .GetAsync<TEntity, Guid>(key, cancellationToken);
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
        public virtual async Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, IOrdering ordering, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            if (ordering == null)
                throw new ArgumentNullException(nameof(ordering));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(where)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<TKey>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            var indent = this.Context.Options.QueryIncludeDepth;

            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(x => keys.Contains(x.Id))
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<int>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.GetManyAsync<TEntity, int>(keys, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<long>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.GetManyAsync<TEntity, long>(keys, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<string>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.GetManyAsync<TEntity, string>(keys, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityIdentity<Guid>
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));

            return await this.GetManyAsync<TEntity, Guid>(keys, cancellationToken);
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
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, IOrdering ordering, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            if (ordering == null)
                throw new ArgumentNullException(nameof(ordering));

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(where)
                .Order(ordering)
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

            if (ordering == null)
                throw new ArgumentNullException(nameof(ordering));

            var indent = this.Context.Options.QueryIncludeDepth;
            
            return await this.GetEntitySet<TEntity>()
                .IncludeAnnotations(indent)
                .Where(where)
                .Limit(pagination)
                .Order(ordering)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, IOrdering ordering, IPagination pagination, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            if (ordering == null)
                throw new ArgumentNullException(nameof(ordering));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            return await this.GetManyAsync(where, pagination, ordering, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = await this.Context
                .AddAsync(entity, cancellationToken);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            entities = entities
                .Select(x => this.Context.Add(x))
                .Select(x => x.Entity)
                .ToArray();

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);

            return entities;
        }

        /// <inheritdoc />
        public virtual async Task AddManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .BulkInsertAsync(entities, x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);
                
            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = this.Context
                .Update(entity);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            entities = entities
                .Select(x => this.Context.Update(x))
                .Select(x => x.Entity)
                .ToArray();

            await this.Context
                .SaveChangesAsync(cancellationToken);

            return entities;
        }

        /// <inheritdoc />
        public virtual async Task UpdateManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.Context
                .BulkUpdateAsync(entities, x =>
                {
                    x.BatchSize = this.Context.Options.BulkBatchSize;
                    x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
                }, cancellationToken);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatableAndUpdatable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = this.Context
                .AddOrUpdate(entity);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);

            return entry.Entity;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatableAndUpdatable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
            entities = entities
                .Select(x => this.Context.AddOrUpdate(x))
                .Select(x => x.Entity)
                .ToArray();

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);

            return entities;
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
    
            this.Context
                .Remove(entity);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            this.Context
                .RemoveRange(entities);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var entities = this.GetEntitySet<TEntity>()
                .Where(criteria);

            this.Context
                .RemoveRange(entities);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var entities = this.GetEntitySet<TEntity>()
                .Where(expression);

            this.Context
                .RemoveRange(entities);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await this.GetEntitySet<TEntity>()
                .BulkDeleteAsync(entities, cancellationToken);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
            where TCriteria : class, IQueryCriteria, new()
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            var entities = this.GetEntitySet<TEntity>()
                .Where(criteria);

            this.Context
                .RemoveRange(entities);

            await this.GetEntitySet<TEntity>()
                .Where(criteria)
                .DeleteAsync(cancellationToken);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task DeleteManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            await this.GetEntitySet<TEntity>()
                .Where(expression)
                .DeleteAsync(cancellationToken);

            if (this.Context.AutoSave)
                await this.SaveChanges(cancellationToken);
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
        public virtual async Task SaveChanges(CancellationToken cancellationToken = default)
        {
            await this.Context
                .SaveChangesAsync(cancellationToken); 
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