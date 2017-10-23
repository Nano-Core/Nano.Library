using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.App.Controllers.Criteria;
using Nano.App.Controllers.Criteria.Extensions;
using Nano.App.Controllers.Criteria.Interfaces;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;
using Nano.Data.Interfaces;

namespace Nano.App.Services
{
    /// <inheritdoc cref="BaseService{TContext}"/>
    public abstract class BaseServiceSpatial<TContext> : BaseService<TContext>, IServiceSpatial
        where TContext : IDbContext
    {
        /// <inheritdoc />
        protected BaseServiceSpatial(TContext context)
            : base(context)
        {

        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Covers<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Covers(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Crosses<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Crosses(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Touches<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Touches(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Overlaps<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Overlaps(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> CoveredBy<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.CoveredBy(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Disjoints<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Disjoint(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Intersects<TEntity, TCriteria>(Query<TCriteria> query, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.Intersects(query.Criteria.Geometry))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(Query<TCriteria> query, double distance = 10000D, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, ICriteriaSpatial
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));
            return await this.Context
                .Set<TEntity>()
                .Where(query.Criteria)
                .Where(x => x.Geometry.IsWithinDistance(query.Criteria.Geometry, distance))
                .Order(query.Order)
                .Limit(query.Paging)
                .ToArrayAsync(cancellationToken);
        }
    }
}