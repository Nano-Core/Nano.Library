using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Controllers.Contracts;
using Nano.App.Models.Interfaces;
using Nano.App.Services.Interfaces;
using Nano.Hosting.Middleware.Extensions;

namespace Nano.App.Controllers
{
    /// <inheritdoc />
    [FormatFilter]
    public abstract class BaseControllerSpatial<TService, TEntity, TIdentity> : BaseController<TService, TEntity, TIdentity>
        where TService : IServiceSpatial
        where TEntity : class, IEntitySpatial, IEntityWritable, IEntityIdentity<TIdentity>
    {
        /// <inheritdoc />
        protected BaseControllerSpatial(ILogger<Controller> logger, TService service)
            : base(logger, service)
        {

        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that intersects the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Intersects([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Intersects<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that covers the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Covers([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Covers<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that are covered By the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> CoveredBy([FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .CoveredBy<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that overlaps the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Overlaps([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Overlaps<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that touches the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Touches([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Touches<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that crosses the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Crosses([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Crosses<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets <see cref="IEntitySpatial"/>'s instances, that disjoints the <paramref name="criteria.Geometry"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="CriteriaSpatial"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Disjoints([FromBody][FromForm][FromQuery][Required]CriteriaSpatial criteria, [FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Disjoints<TEntity>(criteria, paging, cancellationToken);

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
        /// <param name="criteria">The <see cref="CriteriaSpatialWithin"/></param>
        /// <param name="paging">The <see cref="Pagination"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Within([FromBody][FromForm][FromQuery][Required]CriteriaSpatialWithin criteria, [FromForm][FromQuery]Pagination paging = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Within<TEntity>(criteria, paging, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}