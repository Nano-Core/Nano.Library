using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Controllers.Criterias.Entities;
using Nano.Controllers.Criterias.Interfaces;
using Nano.Eventing.Interfaces;
using Nano.Hosting.Extensions;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;

namespace Nano.Controllers
{
    /// <inheritdoc />
    public abstract class BaseControllerSpatial<TService, TEntity, TIdentity, TCriteria> : BaseController<TService, TEntity, TIdentity, TCriteria>
        where TService : IServiceSpatial
        where TEntity : class, IEntitySpatial, IEntityIdentity<TIdentity>, IEntityWritable
        where TCriteria : class, ICriteriaSpatial
    {
        /// <inheritdoc />
        protected BaseControllerSpatial(ILogger logger, TService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that intersects the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Intersects([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Intersects<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that covers the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Covers([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Covers<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are covered By the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> CoveredBy([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .CoveredBy<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that overlaps the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Overlaps([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Overlaps<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that touches the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Touches([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Touches<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that crosses the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Crosses([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Crosses<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that disjoints the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Disjoints([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Disjoints<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Within.
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are within the <paramref name="criteria.Radius"/> of <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Within([FromBody][FromForm][FromQuery][Required]Query<TCriteria> criteria, double distance, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Within<TEntity, TCriteria>(criteria, distance, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}