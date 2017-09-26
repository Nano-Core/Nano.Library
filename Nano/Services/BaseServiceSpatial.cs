using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.Controllers.Contracts;
using Nano.Controllers.Contracts.Extensions;
using Nano.Controllers.Contracts.Interfaces;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Services
{
    /// <inheritdoc cref="BaseService{TContext}"/>
    public abstract class BaseServiceSpatial<TContext> : BaseService<TContext>, IServiceSpatial
        where TContext : DbContext
    {
        /// <inheritdoc />
        protected BaseServiceSpatial(TContext context)
            : base(context)
        {

        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Covers<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Covers(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Crosses<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Crosses(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Touches<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Touches(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Overlaps<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Overlaps(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> CoveredBy<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.CoveredBy(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Disjoints<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Disjoint(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Intersects<T>(ICriteriaSpatial criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.Intersects(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Within<T>(CriteriaSpatialWithin criteria, Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Query(criteria)
                .Where(x => x.Geometry.IsWithinDistance(criteria.Geometry, criteria.Radius))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }
    }
}