using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Web.Const;
using Nano.Web.Models;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = BuiltInUserRoles.Administrator + "," + BuiltInUserRoles.Service + "," + BuiltInUserRoles.Reader)]
    public abstract class BaseControllerSpatialReadOnly<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
        where TRepository : IRepositorySpatial
        where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial
        where TCriteria : class, IQueryCriteriaSpatial, new()
    {
        /// <inheritdoc />
        protected BaseControllerSpatialReadOnly(ILogger logger, TRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }

        /// <summary>
        /// Gets the models, that intersects.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("intersects")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Intersects([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Intersects<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that covers.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("covers")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Covers([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Covers<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that are covered-by.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("covered-by")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CoveredBy([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .CoveredBy<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that are covered-by.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("overlaps")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Overlaps([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Overlaps<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that tocuhes.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("touches")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Touches([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Touches<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that crosses.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("crosses")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Crosses([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Crosses<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that are disjointed.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("disjoints")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Disjoints([FromBody][Required]IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Disjoints<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, that are within.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="distance">The distance in meters. Default: 50.000 meters.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("within")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Within([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]double distance = 50000, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .Within<TEntity, TCriteria>(criteria, distance, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }
    }
}