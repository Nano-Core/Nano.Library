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
using Nano.Data.Extensions;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Repository;

/// <inheritdoc />
public abstract class BaseRepository<TContext> : BaseRepository<TContext, Guid>
    where TContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    protected BaseRepository(TContext context)
        : base(context)
    {
    }
}

/// <inheritdoc />
public abstract class BaseRepository<TContext, TIdentity> : IRepository
    where TContext : BaseDbContext<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Context.
    /// </summary>
    internal virtual TContext Context { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    protected BaseRepository(TContext context)
    {
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public virtual DbContext GetContext()
    {
        return this.Context;
    }

    /// <inheritdoc />
    public virtual DbSet<TEntity> GetEntitySet<TEntity>()
        where TEntity : class, IEntity
    {
        return this.Context.Set<TEntity>();
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        var indent = this.Context.Options.QueryIncludeDepth;

        return this.GetEntitySet<TEntity>()
            .IncludeAnnotations(indent)
            .FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        return this
            .GetAsync<TEntity, int>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        return this
            .GetAsync<TEntity, long>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        return this
            .GetAsync<TEntity, string>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        return this
            .GetAsync<TEntity, Guid>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var indent = this.Context.Options.QueryIncludeDepth;

        return this.GetEntitySet<TEntity>()
            .IncludeAnnotations(indent)
            .Where(query.Criteria)
            .Order(query.Order)
            .Limit(query.Paging)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        var indent = this.Context.Options.QueryIncludeDepth;

        return this.GetEntitySet<TEntity>()
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, int>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, long>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, string>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, Guid>(keys, cancellationToken);
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
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
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
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, CancellationToken cancellationToken = default)
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
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, CancellationToken cancellationToken = default)
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        return this.GetManyAsync(where, pagination, ordering, cancellationToken);
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
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            this.Context
                .Add(entity);
        }

        if (this.Context.AutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        await Task.CompletedTask;
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
        {
            await this.SaveChangesAsync(cancellationToken);
        }
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
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            this.Context
                .Update(entity);
        }

        if (this.Context.AutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        await Task.CompletedTask;
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
        {
            await this.SaveChangesAsync(cancellationToken);
        }
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
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            this.Context
                .AddOrUpdate(entity);
        }

        if (this.Context.AutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        var entity = this.Context.Options.UseSoftDeletetion
            ? await this.Context.FindAsync<TEntity>(id)
            : new TEntity
            {
                Id = id
            };

        await this.DeleteAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync<TEntity>(int id, CancellationToken cancellationToken)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new()
    {
        return this.DeleteAsync<TEntity, int>(id, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync<TEntity>(long id, CancellationToken cancellationToken)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new()
    {
        return this.DeleteAsync<TEntity, long>(id, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync<TEntity>(string id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new()
    {
        return this.DeleteAsync<TEntity, string>(id, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync<TEntity>(Guid id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new()
    {
        return this.DeleteAsync<TEntity, Guid>(id, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        this.Context
            .Remove(entity);

        if (this.Context.AutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        var entities = this.GetEntitySet<TEntity>()
            .Where(x => ids.Contains(x.Id));

        return this.DeleteManyAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        this.Context
            .RemoveRange(entities);

        if (this.Context.AutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteManyAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        var entities = await this.GetManyAsync<TEntity, TCriteria>(new Query<TCriteria>
        {
            Criteria = criteria
        }, cancellationToken);

        await this.DeleteManyAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        var entities = await this.GetManyAsync(expression, cancellationToken);

        await this.DeleteManyAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        var entities = ids
            .Select(x => new TEntity
            {
                Id = x
            });

        return this.DeleteManyBulkAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new()
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return this.DeleteManyBulkAsync<TEntity, Guid>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new()
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return this.DeleteManyBulkAsync<TEntity, int>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new()
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return this.DeleteManyBulkAsync<TEntity, long>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new()
    {
        if (ids == null)
            throw new ArgumentNullException(nameof(ids));

        return this.DeleteManyBulkAsync<TEntity, string>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        return this.GetEntitySet<TEntity>()
            .BulkDeleteAsync(entities, x =>
            {
                x.BatchSize = this.Context.Options.BulkBatchSize;
                x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
            }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        return this.GetEntitySet<TEntity>()
            .Where(criteria)
            .DeleteFromQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        return this.GetEntitySet<TEntity>()
            .Where(expression)
            .DeleteFromQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<long> CountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        return this.GetEntitySet<TEntity>()
            .Where(criteria)
            .LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        return this.GetEntitySet<TEntity>()
            .LongCountAsync(expression, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<decimal> SumAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> sumExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (whereExpr == null)
            throw new ArgumentNullException(nameof(whereExpr));

        if (sumExpr == null)
            throw new ArgumentNullException(nameof(sumExpr));

        return this.GetEntitySet<TEntity>()
            .Where(whereExpr)
            .SumAsync(sumExpr, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<decimal> AverageAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> avgExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (whereExpr == null)
            throw new ArgumentNullException(nameof(whereExpr));

        if (avgExpr == null)
            throw new ArgumentNullException(nameof(avgExpr));

        return this.GetEntitySet<TEntity>()
            .Where(whereExpr)
            .AverageAsync(avgExpr, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
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