using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
    /// <typeparam name="TService">The <see cref="IService"/> inheriting from <see cref="BaseControllerReadOnly{TService,TEntity,TIdentity,TCriteria}"/>.</typeparam>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IService"/> operates with.</typeparam>
    /// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
    [Route("[controller]")]
    public abstract class BaseControllerWritable<TService, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TService, TEntity, TIdentity, TCriteria>
        where TService : IService
        where TEntity : class, IEntity, IEntityIdentity<TIdentity>, IEntityWritable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected BaseControllerWritable(ILogger logger, TService service, IEventing eventing)
            : base(logger, service, eventing)
        {

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
            return this.View("Create");
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
        public virtual async Task<IActionResult> CreateConfirm([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
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
        public virtual async Task<IActionResult> CreateConfirms([FromBody][Required]TEntity[] entities, CancellationToken cancellationToken = default)
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
        [Route("edit/{id}")]
        [Produces(HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Edit([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
        {
            var result = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            return this.View("Edit", result);
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
        public virtual async Task<IActionResult> EditConfirm([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
        {
            TEntity result;
            if (this.Request.Method == WebRequestMethods.Http.Put)
            {
                result = await this.Service.AddOrUpdateAsync(entity, cancellationToken);

            }
            else
            {
                result = await this.Service.UpdateAsync(entity, cancellationToken);
            }

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok(result);
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
        public virtual async Task<IActionResult> EditConfirms([FromBody][Required]TEntity[] entities, CancellationToken cancellationToken = default)
        {
            if (this.Request.Method == WebRequestMethods.Http.Put)
            {
                await this.Service.AddOrUpdateManyAsync(entities.AsEnumerable(), cancellationToken);
            }
            else
            {
                await this.Service.UpdateManyAsync(entities.AsEnumerable(), cancellationToken);
            }

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
        [Route("edit/query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirmsQuery([FromBody][Required]TCriteria select, [FromBody][Required]TEntity update, CancellationToken cancellationToken = default)
        {
            await this.Service.UpdateManyAsync(select, update, cancellationToken);

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
        [Route("delete/{id}")]
        [Produces(HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Delete([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
        {
            var entity = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound();

            return this.View("Delete", entity);
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
        [Route("delete/{id}")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirm([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
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
        public virtual async Task<IActionResult> DeleteConfirms([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
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
        public virtual async Task<IActionResult> DeleteConfirmsQuery([FromBody][Required]TCriteria select, CancellationToken cancellationToken = default)
        {
            await this.Service
                .DeleteManyAsync<TEntity, TCriteria>(select, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }
    }
}