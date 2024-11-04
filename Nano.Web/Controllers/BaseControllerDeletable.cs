using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models;
using Nano.Models.Const;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers;

/// <summary>
/// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TRepository">The <see cref="IRepository"/> inheriting from <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/>.</typeparam>
/// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IRepository"/> operates with.</typeparam>
/// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER)]
public abstract class BaseControllerDeletable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepository
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerDeletable(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

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
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("delete/{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteAsync<TEntity, TIdentity>(id, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Delete the models with the passed identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the models to delete.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("delete/many")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyAsync<TEntity, TIdentity>(ids, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Delete the models with the passed identifiers bulk.
    /// </summary>
    /// <param name="ids">The identifiers of the models to delete.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("delete/many/bulk")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyBulkAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyBulkAsync<TEntity, TIdentity>(ids, cancellationToken);

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteQueryAsync([FromBody][Required]TCriteria select, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyAsync<TEntity, TCriteria>(select, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }
}