using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRepository">The <see cref="IRepository"/> inheriting from <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/>.</typeparam>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IRepository"/> operates with.</typeparam>
    /// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
    [Authorize(Roles = "administrator, service, writer")]
    public abstract class BaseControllerUpdatable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
        where TRepository : IRepository
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected BaseControllerUpdatable(ILogger logger, TRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }
   
        /// <summary>
        /// Gets the 'edit' view for editing a model.
        /// </summary>
        /// <param name="id">The identifier of the model to edit.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The 'edit' view.</returns>
        [HttpGet]
        [Route("edit/{id}")]
        [Produces(HttpContentType.HTML)]
        public virtual async Task<IActionResult> Edit([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            return this.View("Edit", result);
        }

        /// <summary>
        /// Edit the passed model.
        /// </summary>
        /// <param name="entity">The model to edit.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The edited model.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPut]
        [HttpPost]
        [Route("edit")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirm([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
        {
            var result = await this.Repository
                .UpdateAsync(entity, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok(result);
        }

        /// <summary>
        /// Edits the passed models.
        /// </summary>
        /// <param name="entities">The models to edit.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPut]
        [HttpPost]
        [Route("edit/many")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirms([FromBody][Required]TEntity[] entities, CancellationToken cancellationToken = default)
        {
            await this.Repository
                .UpdateManyAsync(entities.AsEnumerable(), cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }

        /// <summary>
        /// Edits the models, changing all entities returned by the passed 'select' criteria, with the values of the passed 'update'.
        /// </summary>
        /// <param name="criteria">The criteria for selecting models to edit, and the model containing values to be changed for all matching entities.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPut]
        [Route("edit/query")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> EditConfirmsQuery([FromBody][Required](TCriteria select, TEntity update) criteria, CancellationToken cancellationToken = default)
        {
            await this.Repository
                .UpdateManyAsync(criteria.select, criteria.update, cancellationToken);

            if (this.Request.IsContentTypeHtml())
                return this.RedirectToAction("Index");

            return this.Ok();
        }
    }
}