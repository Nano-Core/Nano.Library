using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Extensions;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nano.Data;
using Nano.Data.Extensions;
using Nano.Eventing.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Repository;

/// <inheritdoc cref="BaseRepository{TContext, TIdentity}"/>
public abstract class BaseRepositorySpatial<TContext, TIdentity> : BaseRepository<TContext, TIdentity>, IRepositorySpatial
    where TContext : BaseDbContext<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseRepositorySpatial(TContext context, IEventing eventing)
        : base(context, eventing)
    {
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Covers<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Covers(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Crosses<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Crosses(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Touches<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Touches(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Overlaps<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Overlaps(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.CoveredBy<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.CoveredBy(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Disjoints<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Disjoint(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Intersects<TEntity, TCriteria>(query, includeDepth, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.Intersects(query.Criteria.Geometry))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(IQuery<TCriteria> query, double distance = 10000D, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var includeDepth = this.Context.Options.QueryIncludeDepth;

        return this.Within<TEntity, TCriteria>(query, includeDepth, distance, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, double distance = 10000D, CancellationToken cancellationToken = default)
        where TEntity : class, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return await this.Context
            .Set<TEntity>()
            .IncludeAnnotations(includeDepth)
            .Where(query.Criteria)
            .Where(x => x.Geometry.IsWithinDistance(query.Criteria.Geometry, distance))
            .Order(query.Order)
            .Limit(query.Paging)
            .ToArrayAsync(cancellationToken);
    }
}