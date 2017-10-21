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
        public virtual async Task<IEnumerable<TEntity>> Covers<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Covers(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Crosses<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Crosses(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Touches<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Touches(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Overlaps<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Overlaps(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> CoveredBy<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.CoveredBy(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Disjoints<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Disjoint(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Intersects<TEntity, TQuery>(Criteria<TQuery> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TQuery : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.Intersects(criteria.Query.Geometry))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> Within<TEntity, TCriteria>(Criteria<TCriteria> criteria, double distance = 10000D, CancellationToken cancellationToken = default)
            where TEntity : class, IEntitySpatial
            where TCriteria : class, IQuerySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));
            return await this.Context
                .Set<TEntity>()
                .Where(criteria.Query)
                .Where(x => x.Geometry.IsWithinDistance(criteria.Query.Geometry, distance))
                .Order(criteria.Order)
                .Limit(criteria.Paging)
                .ToArrayAsync(cancellationToken);
        }
    }
}