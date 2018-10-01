using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = "administrator, service, writer, reader")]
    public abstract class BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria> : BaseController<TRepository>
        where TRepository : IRepository
        where TEntity : class, IEntityIdentity<TIdentity>
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected BaseControllerReadOnly(ILogger logger, TRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }

        /// <summary>
        /// Gets all models.
        /// Filtered by the parameters in the passed query (pagination and ordering).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>A collection of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("index")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Index([FromQuery][Required]IQuery query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query();

            var result = await this.Repository
                .GetManyAsync<TEntity>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets all models.
        /// Filtered by the parameters in the passed query (pagination and ordering).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>A collection of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("index")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> IndexPost([FromBody][Required]IQuery query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query();

            var result = await this.Repository
                .GetManyAsync<TEntity>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the model.
        /// Uniquely identified by the passed id.
        /// </summary>
        /// <param name="id">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("details/{id}")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.HttpContext.Request.IsContentTypeHtml())
                return this.View("Details", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models.
        /// Uniquely identified by the passed id's.
        /// </summary>
        /// <param name="ids">The identifiers, that uniquely identifies the models.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("details")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DetailsPost([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Details", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets all models.
        /// Filtered by the parameters in the passed query (criteria, pagination and ordering).
        /// </summary>
        /// <param name="query">The query model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>A collection of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("query")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Query([FromQuery][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query<TCriteria>();

            var result = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets all models.
        /// Filtered by the parameters in the passed query (criteria, pagination and ordering).
        /// </summary>
        /// <param name="query">The query model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>A collection of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("query")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryPost([FromBody][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query<TCriteria>();

            var result = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}