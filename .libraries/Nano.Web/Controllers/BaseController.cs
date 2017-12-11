using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
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
    /// <typeparam name="TCriteria"></typeparam>
    public abstract class BaseController<TService, TEntity, TIdentity, TCriteria> : Controller
        where TService : IService
        where TEntity : class, IEntity, IEntityIdentity<TIdentity>, IEntityWritable
        where TCriteria : class, IQueryCriteria
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

            context.Result = this.BadRequest(new
            {
                Summary = "Invalid ModelState",
                Errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToArray()
            });
        }

        /// <summary>
        /// Index.
        /// Gets all instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="query">The <see cref="Query"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Index([FromQuery][FromBody][FromForm][Required]Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAll<TEntity>(query ?? new Query(), cancellationToken);

            if (this.Response.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Details.
        /// Gets details about an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The <typeparamref name="TIdentity"/> identifier of the <typeparamref name="TEntity"/> instance to get details about.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Details' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Details([FromRoute][FromQuery][FromBody][FromForm][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Get<TEntity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Details.
        /// Gets details about an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="ids">The <typeparamref name="TIdentity"/> identifiers of the <typeparamref name="TEntity"/> instances to get details about.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Details' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Details([FromRoute][FromQuery][FromBody][FromForm][Required]TIdentity[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Get<TEntity>(ids, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.Ok(result);
        }

        /// <summary>
        /// Query.
        /// Queries instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="Query{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [HttpPost]
        public virtual async Task<IActionResult> Query([FromQuery][FromBody][FromForm][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetMany<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Create.
        /// Returns 'Create' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Create' <see cref="IActionResult"/></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Create(CancellationToken cancellationToken = new CancellationToken())
        {
            return await Task.Factory
                .StartNew<IActionResult>(x =>
                {
                    if (this.Request.IsContentTypeHtml())
                        return this.View();

                    return this.NoContent();
                }, null, cancellationToken);
        }

        /// <summary>
        /// Create.
        /// Creates an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody][FromForm][Required]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .Add(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.CreatedAtAction("Create", entity);
        }

        /// <summary>
        /// Creates.
        /// Creates instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entities">The <see cref="IEntity"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Creates([FromBody][FromForm][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .AddMany(entities, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.CreatedAtAction("Creates", entities);
        }

        /// <summary>
        /// Edit.
        /// Returns 'Edit' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Edit' <see cref="IActionResult"/></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Edit([FromRoute][FromQuery][Required]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .Get<TEntity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View(result);

            return this.NoContent();
        }

        /// <summary>
        /// Edit.
        /// Edit an instance of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="entity">The <see cref="IEntity"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPut]
        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromRoute][FromQuery][Required]Guid id, [FromBody][FromForm][Required]TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .Update(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok(entity);
        }

        /// <summary>
        /// Edits.
        /// Edits instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="entities">The <see cref="IEntity"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPut]
        [HttpPost]
        public virtual async Task<IActionResult> Edits([FromBody][FromForm][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            await this.Service
                .UpdateMany(entities, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok(entities);
        }

        /// <summary>
        /// Delete.
        /// Returns 'Delete' view for the model of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Delete' <see cref="IActionResult"/>.</returns>
        [HttpGet]
        public virtual async Task<IActionResult> Delete([FromRoute][FromQuery][Required]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var entity = await this.Service
                .Get<TEntity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound(id);

            if (this.Request.IsContentTypeHtml())
                return this.View(entity);

            return this.NoContent();
        }

        /// <summary>
        /// Delete.
        /// Deletes the instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Delete' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [HttpDelete]
        [ActionName("Delete")]
        public virtual async Task<IActionResult> DeleteConfirm([FromRoute][FromQuery][FromBody][FromForm][Required]Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            var entity = await this
                .Service.Get<TEntity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound(id);

            await this.Service
                .Delete(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.AcceptedAtAction("Delete", entity);
        }

        /// <summary>
        /// Deletes.
        /// Deletes instances of <see cref="IEntity"/> of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="ids">The Id's.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The 'Index' <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [HttpDelete]
        [ActionName("Deletes")]
        public virtual async Task<IActionResult> DeleteConfirms([FromBody][FromForm][Required]IEnumerable<TIdentity> ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = await this
                .Service.GetMany<TEntity>(x => ids.Contains(x.Id), cancellationToken);

            await this.Service
                .DeleteMany(entities, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.AcceptedAtAction("Deletes", entities);
        }
    }
}