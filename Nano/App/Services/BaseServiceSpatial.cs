using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.App.Controllers.Contracts;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;
using Nano.Data.Interfaces;
using Nano.Data.Extensions;

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
        public virtual async Task<IEnumerable<T>> Covers<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Covers(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Crosses<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Crosses(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Touches<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Touches(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Overlaps<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Overlaps(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> CoveredBy<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.CoveredBy(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Disjoints<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Disjoint(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Intersects<T>(CriteriaSpatial criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.Intersects(criteria.Geometry))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> Within<T>(CriteriaSpatialWithin criteria, Pagination paging = default, CancellationToken cancellationToken = default)
            where T : class, IEntitySpatial
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            paging = paging ?? new Pagination();

            return await this.Context
                .Set<T>()
                .Where(criteria)
                .Where(x => x.Geometry.IsWithinDistance(criteria.Geometry, criteria.Radius))
                .Skip(paging.Skip)
                .Take(paging.Count)
                .ToArrayAsync(cancellationToken);
        }
    }
}