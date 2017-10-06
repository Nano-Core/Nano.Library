using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Contracts;
using Nano.App.Controllers.Contracts.Interfaces;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;
using Nano.Hosting.Middleware.Extensions;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    public abstract class BaseControllerSpatial<TService, TEntity, TIdentity, TCriteria> : BaseController<TService, TEntity, TIdentity, TCriteria>
        where TService : IServiceSpatial
        where TEntity : class, IEntitySpatial, IEntityWritable, IEntityIdentity<TIdentity>
        where TCriteria : class, ICriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected BaseControllerSpatial(ILogger<Controller> logger, TService service)
            : base(logger, service)
        {

        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that intersects the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Intersects([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Intersects<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that covers the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Covers([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Covers<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are covered By the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> CoveredBy([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .CoveredBy<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that overlaps the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Overlaps([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Overlaps<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that touches the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Touches([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Touches<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that crosses the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Crosses([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Crosses<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that disjoints the <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Disjoints([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Disjoints<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Within.
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are within the <paramref name="query.Radius"/> of <paramref name="query.Geometry"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Within([FromBody][FromForm][FromQuery][Required]Query<TCriteria> query, double distance, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Within<TEntity, TCriteria>(query, distance, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}