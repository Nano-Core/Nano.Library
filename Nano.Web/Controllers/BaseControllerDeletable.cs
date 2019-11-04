using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    /// <summary>
    /// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TRepository">The <see cref="IRepository"/> inheriting from <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/>.</typeparam>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IRepository"/> operates with.</typeparam>
    /// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
    [Authorize(Roles = BuiltInUserRoles.Administrator + "," + BuiltInUserRoles.Service + "," + BuiltInUserRoles.Writer)]
    public abstract class BaseControllerDeletable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
        where TRepository : IRepository
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected BaseControllerDeletable(ILogger logger, TRepository repository, IEventing eventing)
            : base(logger, repository, eventing)
        {

        }

        /// <summary>
        /// Delete the model with the passed identifier.
        /// </summary>
        /// <param name="id">The identifier of the model to delete.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete/{id}")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirm([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
        {
            var entity = await this
                .Repository.GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (entity == null)
                return this.NotFound();

            await this.Repository
                .DeleteAsync(entity, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Deletes the models with the passed identifier's.
        /// </summary>
        /// <param name="ids">The identifiers of the models to delete.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete/many")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirms([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
        {
            var paging = new Pagination
            {
                Count = 1,
                Number = int.MaxValue
            };

            var entities = await this
                .Repository.GetManyAsync<TEntity>(x => ids.Contains(x.Id), paging, cancellationToken);

            if (entities == null)
                return this.NotFound();

            await this.Repository
                .DeleteManyAsync(entities, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Deletes the models matching the passed 'select' criteria.
        /// </summary>
        /// <param name="select">The crtieria for selecting models to delete.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [HttpDelete]
        [Route("delete/query")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> DeleteConfirmsQuery([FromBody][Required]TCriteria select, CancellationToken cancellationToken = default)
        {
            await this.Repository
                .DeleteManyAsync<TEntity, TCriteria>(select, cancellationToken);

            return this.Ok();
        }
    }
}