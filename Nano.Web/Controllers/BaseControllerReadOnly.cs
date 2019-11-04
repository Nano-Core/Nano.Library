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
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Web.Const;
using Nano.Web.Models;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    [Authorize(Roles = BuiltInUserRoles.Administrator + "," + BuiltInUserRoles.Service + "," + BuiltInUserRoles.Writer + "," + BuiltInUserRoles.Reader)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
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

            return this.Ok(result);
        }
        
        /// <summary>
        /// Gets first entity mathcing the passed query (criteria, pagination and ordering).
        /// </summary>
        /// <param name="query">The query model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The model, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("query/first")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryFirst([FromQuery][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query<TCriteria>();

            var result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }
        
        /// <summary>
        /// Gets first entity mathcing the passed query (criteria, pagination and ordering).
        /// </summary>
        /// <param name="query">The query model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The model, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("query/first")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryFirstPost([FromQuery][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        {
            query = query ?? new Query<TCriteria>();

            var result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the number of models (Count).
        /// </summary>
        /// <param name="criteria">The criteria model, containing filters used in the criteria.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The count of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("query/count")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryCount([FromQuery][Required]TCriteria criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the number of models (Count).
        /// </summary>
        /// <param name="criteria">The criteria model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The count of models, matching the passed query.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("query/count")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryCountPost([FromBody][Required]TCriteria criteria, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

            return this.Ok(result);
        }
    }
}