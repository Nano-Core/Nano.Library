using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Extensions;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nano.Data;
using Nano.Data.Extensions;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Repository;

/// <inheritdoc />
public abstract class BaseRepository<TContext> : BaseRepository<TContext, Guid>
    where TContext : BaseDbContext<Guid>
{
    /// <inheritdoc />
    protected BaseRepository(TContext context, IEventing eventing)
        : base(context, eventing)
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
    /// Eventing.
    /// </summary>
    internal virtual IEventing Eventing { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/>.</param>
    /// <param name="eventing">The <see cref="IEventing"/>.</param>
    protected BaseRepository(TContext context, IEventing eventing)
    {
        this.Context = context ?? throw new ArgumentNullException(nameof(context));
        this.Eventing = eventing ?? throw new ArgumentNullException(nameof(eventing));
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
    public virtual Task<TEntity> GetAsync<TEntity, TKey>(TKey key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        return this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetAsync<TEntity, TKey>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        return this
            .GetAsync<TEntity, int>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(int key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        return this
            .GetAsync<TEntity, int>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        return this
            .GetAsync<TEntity, long>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(long key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        return this
            .GetAsync<TEntity, long>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        return this
            .GetAsync<TEntity, string>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(string key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        return this
            .GetAsync<TEntity, string>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        return this
            .GetAsync<TEntity, Guid>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetAsync<TEntity>(Guid key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        return this
            .GetAsync<TEntity, Guid>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetFirstAsync<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Order(query.Order)
            .Limit(query.Paging)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetFirstAsync(where, new Ordering(), includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetFirstAsync(where, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        return this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Order(ordering)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync<TEntity, TKey>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, int>(keys, includeDepth, cancellationToken);
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, long>(keys, includeDepth, cancellationToken);
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, string>(keys, includeDepth, cancellationToken);
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return this.GetManyAsync<TEntity, Guid>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync<TEntity>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Order(ordering)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, pagination, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Limit(pagination)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, pagination, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        return await this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
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
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (ordering == null)
            throw new ArgumentNullException(nameof(ordering));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        return this.GetManyAsync(where, pagination, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default) 
        where TEntity : class, IEntity
    {
        if (where == null) 
            throw new ArgumentNullException(nameof(where));
        
        if (orderBy == null) 
            throw new ArgumentNullException(nameof(orderBy));
        
        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, orderBy, includeDepth, orderingDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (orderBy == null)
            throw new ArgumentNullException(nameof(orderBy));

        return this.GetManyAsync(where, orderBy, new Pagination(), includeDepth, orderingDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default) 
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (orderBy == null)
            throw new ArgumentNullException(nameof(orderBy));

        if (pagination == null)
            throw new ArgumentNullException(nameof(pagination));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.GetManyAsync(where, orderBy, pagination, includeDepth, orderingDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (where == null)
            throw new ArgumentNullException(nameof(where));

        if (orderBy == null)
            throw new ArgumentNullException(nameof(orderBy));

        await Task.CompletedTask;

        var entities = this.GetEntitySet<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Limit(pagination)
            .AsEnumerable();

        return orderingDirection switch
        {
            OrderingDirection.Asc => entities
                .OrderBy(orderBy)
                .ToArray(),
            OrderingDirection.Desc => entities
                .OrderBy(orderBy)
                .ToArray(),
            _ => entities
        };
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
    public virtual async Task<TEntity> AddAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        entity = await this.AddAsync(entity, cancellationToken);

        return await this.GetAsync<TEntity, TKey>(entity.Id, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        await this.Context
            .AddRangeAsync(entities, cancellationToken);

        if (this.Context.AutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual Task AddManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        return this.Context
            .BulkInsertAsync(entities, x =>
            {
                x.BatchSize = this.Context.Options.BulkBatchSize;
                x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
            }, cancellationToken);
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
    public virtual async Task<TEntity> UpdateAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        var includeDepth = this.Context.Options.QueryIncludeDepth;

        entity = await this.UpdateAsync(entity, cancellationToken);

        return await this.GetAsync<TEntity, TKey>(entity.Id, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        this.Context
            .UpdateRange(entities);

        if (this.Context.AutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        return this.Context
            .BulkUpdateAsync(entities, x =>
            {
                x.BatchSize = this.Context.Options.BulkBatchSize;
                x.BatchDelayInterval = this.Context.Options.BulkBatchDelay;
            }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        if (criteria == null) 
            throw new ArgumentNullException(nameof(criteria));
        
        if (propertyUpdates == null) 
            throw new ArgumentNullException(nameof(propertyUpdates));

        var updateExpression = this.BuildUpdateExpression<TEntity>(propertyUpdates);

        return this.GetEntitySet<TEntity>()
            .Where(criteria)
            .ExecuteUpdateAsync(updateExpression, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> where, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        if (where == null) 
            throw new ArgumentNullException(nameof(where));

        if (propertyUpdates == null)
            throw new ArgumentNullException(nameof(propertyUpdates));

        var updateExpression = this.BuildUpdateExpression<TEntity>(propertyUpdates);

        return this.GetEntitySet<TEntity>()
            .Where(where)
            .ExecuteUpdateAsync(updateExpression, cancellationToken);
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
    public virtual Task DeleteAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        var entity = this.Context.Options.UseSoftDeletetion
            ? this.Context.Find<TEntity>(id)
            : new TEntity
            {
                Id = id
            };

        return this.DeleteAsync(entity, cancellationToken);
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

    private Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> BuildUpdateExpression<TEntity>(Dictionary<string, object> updates)
        where TEntity : class
    {
        if (updates == null) 
            throw new ArgumentNullException(nameof(updates));
        
        var parameter = Expression.Parameter(typeof(SetPropertyCalls<TEntity>), "instance");

        Expression expression = parameter;
        foreach (var (propertyName, value) in updates)
        {
            var entityParameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(entityParameter, propertyName);

            var propertyType = property.Type;
            var genericType = typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType);
            var propertyLambda = Expression.Lambda(genericType, property, entityParameter);
            var constantValue = Expression.Constant(value, propertyType);
            var valueLambda = Expression.Lambda(genericType, constantValue, entityParameter);

            var setPropertyMethod = typeof(SetPropertyCalls<TEntity>)
                .GetMethods()
                .FirstOrDefault(x =>
                    x.Name == "SetProperty" &&
                    x.IsGenericMethod &&
                    x.GetParameters().Length == 2 &&
                    x.GetParameters()[0].ParameterType.GetGenericArguments()[0] == typeof(TEntity));

            if (setPropertyMethod == null)
            {
                throw new NullReferenceException(nameof(setPropertyMethod));
            }

            var genericSetPropertyMethod = setPropertyMethod
                .MakeGenericMethod(propertyType);

            expression = Expression.Call(expression, genericSetPropertyMethod, propertyLambda, valueLambda);
        }

        return Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(expression, parameter);
    }
}