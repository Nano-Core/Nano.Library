using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Extensions;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data;

/// <inheritdoc />
public abstract class BaseRepository<TContext, TIdentity> : IRepository
    where TContext : BaseDbContext<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IOptionsMonitor<DataOptions> options;
    private readonly TContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRepository{TContext, TIdentity}"/> class.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{DataOptions}"/> used to access repository configuration.</param>
    /// <param name="context">The database context.</param>
    protected BaseRepository(IOptionsMonitor<DataOptions> options, TContext context)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.dbContext = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        return this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .FirstOrDefaultAsync(x => x.Id.Equals(key), cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetAsync<TEntity, TKey>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        return this.GetAsync<TEntity, int>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(int key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        return this.GetAsync<TEntity, int>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        return this.GetAsync<TEntity, long>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(long key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        return this.GetAsync<TEntity, long>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        ArgumentNullException.ThrowIfNull(key);

        return this.GetAsync<TEntity, string>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(string key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        ArgumentNullException.ThrowIfNull(key);

        return this.GetAsync<TEntity, string>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        return this.GetAsync<TEntity, Guid>(key, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetAsync<TEntity>(Guid key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        return this.GetAsync<TEntity, Guid>(key, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetFirstAsync<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Order(query.Order)
            .Limit(query.Paging)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetFirstAsync(where, new Ordering(), includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);

        return this.GetFirstAsync(where, new Ordering(), includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetFirstAsync(where, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);

        return this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Order(ordering)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(keys);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync<TEntity, TKey>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return await this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(x => keys.Contains(x.Id))
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, int>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, int>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, long>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, long>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, string>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, string>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, Guid>(keys, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>
    {
        ArgumentNullException.ThrowIfNull(keys);

        return this.GetManyAsync<TEntity, Guid>(keys, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(query);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync<TEntity>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(query);

        return await this.dbContext
            .Set<TEntity>()
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
        ArgumentNullException.ThrowIfNull(query);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return await this.dbContext
            .Set<TEntity>()
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
        ArgumentNullException.ThrowIfNull(where);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);

        return await this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);

        return await this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Order(ordering)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(pagination);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, pagination, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(pagination);

        return await this.dbContext
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(where)
            .Limit(pagination)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(pagination);
        ArgumentNullException.ThrowIfNull(ordering);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, pagination, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(pagination);
        ArgumentNullException.ThrowIfNull(ordering);

        return await this.dbContext
            .Set<TEntity>()
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
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);
        ArgumentNullException.ThrowIfNull(pagination);

        return this.GetManyAsync(where, pagination, ordering, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(ordering);
        ArgumentNullException.ThrowIfNull(pagination);

        return this.GetManyAsync(where, pagination, ordering, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, OrderingDirection orderDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(orderBy);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, orderBy, includeDepth, orderDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(orderBy);

        return this.GetManyAsync(where, orderBy, new Pagination(), includeDepth, orderingDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(orderBy);
        ArgumentNullException.ThrowIfNull(pagination);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        return this.GetManyAsync(where, orderBy, pagination, includeDepth, orderingDirection, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(orderBy);

        await Task.CompletedTask;

        var entities = this.dbContext
            .Set<TEntity>()
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
        ArgumentNullException.ThrowIfNull(entity);

        var entry = await this.dbContext
            .AddAsync(entity, cancellationToken);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> AddAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(entity);

        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        var entry = await this.dbContext
            .AddAsync(entity, cancellationToken);

        await this.SaveChangesAsync(cancellationToken);

        return await this.GetAsync<TEntity, TKey>(entry.Entity.Id, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        await this.dbContext
            .AddRangeAsync(entities, cancellationToken);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public virtual Task AddManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.dbContext
            .BulkInsertAsync(entities, x =>
            {
                x.BatchSize = this.options.CurrentValue.BulkBatchSize;
                x.BatchDelayInterval = this.options.CurrentValue.BulkBatchDelay;
            }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = this.dbContext
            .Update(entity);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> UpdateAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>
    {
        var includeDepth = this.options.CurrentValue.Repository.QueryIncludeDepth;

        var entry = this.dbContext
            .Update(entity);

        await this.SaveChangesAsync(cancellationToken);

        return await this.GetAsync<TEntity, TKey>(entry.Entity.Id, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.dbContext
            .UpdateRange(entities);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateManyAsync<TEntity, TCriteria>(TCriteria criteria, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        var updater = BuildUpdateExpression<TEntity>(propertyUpdates).Compile();

        var entities = this.dbContext
            .Set<TEntity>()
            .Where(criteria)
            .ToArray();

        foreach (var entity in entities)
        {
            updater(entity);
        }

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        var updater = BuildUpdateExpression<TEntity>(propertyUpdates).Compile();

        var entities = this.dbContext
            .Set<TEntity>()
            .Where(where)
            .ToArray();

        foreach (var entity in entities)
        {
            updater(entity);
        }

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            return this.SaveChangesAsync(cancellationToken);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.dbContext
            .BulkUpdateAsync(entities, x =>
            {
                x.BatchSize = this.options.CurrentValue.BulkBatchSize;
                x.BatchDelayInterval = this.options.CurrentValue.BulkBatchDelay;
            }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        var updateExpression = BuildBulkUpdateExpression<TEntity>(propertyUpdates);

        return this.dbContext
            .Set<TEntity>()
            .Where(criteria)
            .ExecuteUpdateAsync(updateExpression, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task UpdateManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> where, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(where);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        var updateExpression = BuildBulkUpdateExpression<TEntity>(propertyUpdates);

        return this.dbContext
            .Set<TEntity>()
            .Where(where)
            .ExecuteUpdateAsync(updateExpression, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entry = this.dbContext
            .AddOrUpdate(entity);

        if (this.options.CurrentValue.Repository.UseAutoSave)
        {
            await this.SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public virtual async Task AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            this.dbContext
                .AddOrUpdate(entity);
        }

        if (this.options.CurrentValue.Repository.UseAutoSave)
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
        var entity = this.options.CurrentValue.UseSoftDeletetion
            ? await this.dbContext.FindAsync<TEntity>([id], cancellationToken)
            : new TEntity
            {
                Id = id
            };

        if (entity == null)
        {
            throw new NullReferenceException(nameof(entity));
        }

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
        ArgumentNullException.ThrowIfNull(id);

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
        ArgumentNullException.ThrowIfNull(entity);

        this.dbContext
            .SingleDelete(entity);

        if (this.options.CurrentValue.Repository.UseAutoSave)
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
        ArgumentNullException.ThrowIfNull(ids);

        var entities = this.dbContext
            .Set<TEntity>()
            .Where(x => ids.Contains(x.Id));

        return this.DeleteManyAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyAsync<TEntity, Guid>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyAsync<TEntity, int>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyAsync<TEntity, long>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyAsync<TEntity, string>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(entities);

        this.dbContext
            .RemoveRange(entities);

        if (this.options.CurrentValue.Repository.UseAutoSave)
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
        ArgumentNullException.ThrowIfNull(criteria);

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
        ArgumentNullException.ThrowIfNull(expression);

        var entities = await this.GetManyAsync(expression, cancellationToken);

        await this.DeleteManyAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(ids);

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
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyBulkAsync<TEntity, Guid>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyBulkAsync<TEntity, int>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyBulkAsync<TEntity, long>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new()
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyBulkAsync<TEntity, string>(ids, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.dbContext
            .Set<TEntity>()
            .BulkDeleteAsync(entities, x =>
            {
                x.BatchSize = this.options.CurrentValue.BulkBatchSize;
                x.BatchDelayInterval = this.options.CurrentValue.BulkBatchDelay;
            }, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.dbContext
            .Set<TEntity>()
            .Where(criteria)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task DeleteManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(expression);

        return this.dbContext
            .Set<TEntity>()
            .Where(expression)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<long> CountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.dbContext
            .Set<TEntity>()
            .Where(criteria)
            .LongCountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(expression);

        return this.dbContext
            .Set<TEntity>()
            .LongCountAsync(expression, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<decimal> SumAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> sumExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(whereExpr);
        ArgumentNullException.ThrowIfNull(sumExpr);

        return this.dbContext
            .Set<TEntity>()
            .Where(whereExpr)
            .SumAsync(sumExpr, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<decimal> AverageAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> avgExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(whereExpr);
        ArgumentNullException.ThrowIfNull(avgExpr);

        return this.dbContext
            .Set<TEntity>()
            .Where(whereExpr)
            .AverageAsync(avgExpr, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.dbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisposable" />
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        this.dbContext
            .Dispose();
    }


    private static Expression<Action<TEntity>> BuildUpdateExpression<TEntity>(Dictionary<string, object> updates)
    {
        ArgumentNullException.ThrowIfNull(updates);

        var entityParam = Expression.Parameter(typeof(TEntity), "x");

        var expressions = new List<Expression>();

        foreach (var (propertyName, value) in updates)
        {
            var property = Expression.Property(entityParam, propertyName);
            var constant = Expression.Constant(value, property.Type);

            var assign = Expression.Assign(property, constant);

            expressions
                .Add(assign);
        }

        var body = Expression.Block(expressions);

        return Expression.Lambda<Action<TEntity>>(body, entityParam);
    }
    private static Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> BuildBulkUpdateExpression<TEntity>(Dictionary<string, object> updates)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(updates);

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
                    x is { Name: "SetProperty", IsGenericMethod: true } &&
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