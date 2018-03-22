using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TService">The <see cref="IService"/> inheriting from <see cref="BaseController{TService,TEntity,TIdentity, TCriteria}"/>.</typeparam>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IService"/> operates with.</typeparam>
    /// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
    [Route("[controller]")]
    public abstract class BaseController<TService, TEntity, TIdentity, TCriteria> : Controller
        where TService : IService
        where TEntity : class, IEntity, IEntityIdentity<TIdentity>, IEntityWritable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Service.
        /// </summary>
        protected virtual TService Service { get; }
        
        /// <summary>
        /// Eventing.
        /// </summary>
        protected virtual IEventing Eventing { get; }
        
        /// <summary>
        /// Constructor accepting an instance of <typeparamref name="TService"/> and initializing <see cref="Service"/>
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="service">The <see cref="IService"/>.</param>
        /// <param name="eventing">The <see cref="IEventingProvider"/>.</param>
        protected BaseController(ILogger logger, TService service, IEventing eventing)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (eventing == null)
                throw new ArgumentNullException(nameof(eventing));

            this.Logger = logger;
            this.Service = service;
            this.Eventing = eventing;
        }

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.ModelState.IsValid)
                return;

            context.Result = this.BadRequest(new Error
            {
                Summary = "Invalid ModelState",
                Errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToArray()
            });
        }

        /// <summary>
        /// Gets all models. 
        /// Filtered by query model parameters (pagination and ordering).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>A collection of models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [HttpPost]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Index([FromQuery][FromBody][FromForm]Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAllAsync<TEntity>(query ?? new Query(), cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the model, uniquely identified by the supplied identifier.
        /// </summary>
        /// <param name="id">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Details about the model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [Route("details")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromRoute][FromQuery][FromBody][FromForm][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, uniquely identified by the supplied array of identifiers.
        /// </summary>
        /// <param name="ids">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpPost]
        [Route("details")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Detailss([FromBody][FromForm][Required]TIdentity[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, matching the query criteria, pagination and ordering.
        /// </summary>
        /// <param name="criteria">The criteria model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpGet]
        [Route("query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Query([FromQuery][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetManyAsync<TEntity, TCriteria>(criteria, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, matching the query criteria, pagination and ordering.
        /// </summary>
        /// <param name="criteria">The criteria model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpPost]
        [Route("query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> QueryPost([FromBody][FromForm][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetManyAsync<TEntity, TCriteria>(criteria, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }


        /// <summary>
        /// Gets the view for creating a new model.
        /// </summary>
        /// <returns>The view.</returns>
        /// <response code="200">Success.</response>
        [HttpGet]
        [Route("create")]
        [Produces(HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        public virtual IActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        /// Create the model.
        /// </summary>
        /// <param name="entity">The model to create.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The created model.</returns>
        /// <response code="201">Created successfully.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [Route("create")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CreateConfirm([FromBody][FromForm][Required]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .AddAsync(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Created("Create", result);
        }

        /// <summary>
        /// Creates the models.
        /// </summary>
        /// <param name="entities">The model to create.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="201">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [Route("create/Many")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> CreateConfirms([FromBody][FromForm][Required]TEntity[] entities, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .AddManyAsync(entities, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Created("Create/Many", entities);
        }

        /// <summary>
        /// Gets the view for editing an existing model.
        /// </summary>
        /// <returns>The view.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found, when getting the model to edit.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpGet]
        [Route("edit")]
        [Produces(HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Edit([FromRoute][FromQuery][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.View(result);
        }

        /// <summary>
        /// Edit the model.
        /// </summary>
        /// <param name="entity">The model to edit.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The edited model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPut]
        [HttpPost]
        [Route("edit")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirm([FromBody][FromForm][Required]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .UpdateAsync(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok(entity);
        }

        /// <summary>
        /// Edits the models.
        /// </summary>
        /// <param name="entities">The models to edit.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothong (void).</returns>
        /// <response code="201">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPut]
        [HttpPost]
        [Route("edit/many")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirms([FromBody][FromForm][Required]TEntity[] entities, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .UpdateManyAsync(entities.AsEnumerable(), cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Edits the models returned by the select criteria.
        /// </summary>
        /// <param name="select">The crtieria for selecting models to edit.</param>
        /// <param name="update">The model, of which to edit all selected entities by.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPut]
        [HttpPost]
        [Route("edit/query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirmQuery([FromBody][FromForm][Required]TCriteria select, [FromBody][FromForm][Required]TEntity update, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .UpdateManyAsync(select, update, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Gets the view for deleting an existing model.
        /// </summary>
        /// <returns>The view.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found, when getting the model to delete.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpGet]
        [Route("delete")]
        [Produces(HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Delete([FromRoute][FromQuery][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var entity = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound();

            return this.View(entity);
        }

        /// <summary>
        /// Delete the model.
        /// </summary>
        /// <param name="id">The identifier of the model to delete.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found, when getting the model to delete.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirm([FromRoute][FromQuery][FromBody][FromForm][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var entity = await this
                .Service.GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound(id);

            await this.Service
                .DeleteAsync(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Deletes the models.
        /// </summary>
        /// <param name="ids">The models to delete.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete/many")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirms([FromBody][FromForm][Required]TIdentity[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = await this
                .Service.GetManyAsync<TEntity>(x => ids.Contains(x.Id), cancellationToken);

            await this.Service
                .DeleteManyAsync(entities, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Deletes the models matching the select criteria.
        /// </summary>
        /// <param name="select">The crtieria for selecting models to delete.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Nothing (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete/query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirmQuery([FromBody][FromForm][Required]TCriteria select, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .DeleteManyAsync<TEntity, TCriteria>(select, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }
    }
}