using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Controllers;

/// <summary>
/// Base generic API controller that provides repository and eventing support.
/// </summary>
public abstract class BaseEntityController : BaseController
{
    /// <summary>
    /// Repository used for data access operations within the controller.
    /// </summary>
    protected virtual IRepository Repository { get; }

    /// <summary>
    /// Optional eventing interface for publishing domain events.
    /// </summary>
    protected virtual IEventing? Eventing { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityController"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used for logging.</param>
    /// <param name="repository">The repository implementing <see cref="IRepository"/>.</param>
    /// <param name="eventing">Optional <see cref="IEventing"/> for publishing events.</param>
    protected BaseEntityController(ILogger<BaseEntityController> logger, IRepository repository, IEventing? eventing = null)
        : base(logger)
    {
        this.Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.Eventing = eventing;
    }
}

/// <inheritdoc />
public abstract class BaseEntityController<TEntity, TCriteria> : BaseEntityController<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseEntityController(ILogger<BaseEntityController<TEntity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <summary>
/// Controller providing readable and writable operations (Create, Edit, Delete).
/// </summary>
/// <typeparam name="TEntity">The entity type implementing <see cref="IEntity"/> handled by this controller.</typeparam>
/// <typeparam name="TIdentity">The identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.DELETER)]
public abstract class BaseEntityController<TEntity, TIdentity, TCriteria> : BaseEntityCreatableAndEditableController<TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseEntityController(ILogger<BaseEntityController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Deletes a single entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entity deleted.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Entity not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.DELETE)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteAsync<TEntity, TIdentity>(id, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Deletes multiple entities by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities deleted.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Entities not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.DELETE_MANY)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyAsync<TEntity, TIdentity>(ids, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Deletes multiple entities by their identifiers in bulk.
    /// </summary>
    /// <param name="ids">The identifiers of the entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities deleted.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Entities not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.DELETE_MANY_BULK)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyBulkAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyBulkAsync<TEntity, TIdentity>(ids, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Deletes entities matching the specified criteria.
    /// </summary>
    /// <param name="select">The criteria for selecting entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities deleted.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.DELETE_QUERY)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteQueryAsync([FromBody][Required] TCriteria select, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyAsync<TEntity, TCriteria>(select, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Bulk deletes entities matching the specified criteria.
    /// </summary>
    /// <param name="select">The criteria for selecting entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities deleted.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.DELETE_QUERY_BULK)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteQueryBulkAsync([FromBody][Required] TCriteria select, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .DeleteManyBulkAsync<TEntity, TCriteria>(select, cancellationToken);

        return this.Ok();
    }
}