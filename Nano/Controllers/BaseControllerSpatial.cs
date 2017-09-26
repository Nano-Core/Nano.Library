using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Contracts;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Controllers
{
    /// <inheritdoc />
    public abstract class BaseControllerSpatial<TService, TEntity> : BaseController<TService, TEntity>
        where TService : IServiceSpatial
        where TEntity : class, IEntitySpatial, IEntityWritable
    {
        /// <inheritdoc />
        protected BaseControllerSpatial(ILoggerFactory loggerFactory, TService service)
            : base(loggerFactory, service)
        {

        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that intersects the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("intersects")]
        public virtual async Task<IEnumerable<TEntity>> Intersects([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Intersects<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that covers the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("Covers")]
        public virtual async Task<IEnumerable<TEntity>> Covers([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Covers<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are covered By the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("coveredBy")]
        public virtual async Task<IEnumerable<TEntity>> CoveredBy([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .CoveredBy<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that overlaps the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("overlaps")]
        public virtual async Task<IEnumerable<TEntity>> Overlaps([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Overlaps<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that touches the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("touches")]
        public virtual async Task<IEnumerable<TEntity>> Touches([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Touches<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that crosses the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("crosses")]
        public virtual async Task<IEnumerable<TEntity>> Crosses([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Crosses<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that disjoints the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("disjoints")]
        public virtual async Task<IEnumerable<TEntity>> Disjoints([FromQuery]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Disjoints<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are within the <paramref name="criteria.Radius"/> of <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/>.</returns>
        [HttpGet("within")]
        public virtual async Task<IEnumerable<TEntity>> Within([FromQuery]CriteriaSpatialWithin criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            using (this.Logger.BeginScope(this.HttpContext.Session.Id))
            {
                try
                {
                    return await this.Service
                        .Within<TEntity>(criteria, paging, cancellationToken);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message, ex);
                    throw;
                }
            }
        }
    }
}