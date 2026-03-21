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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseEntityCreatableController<TEntity, TCriteria> : BaseEntityCreatableController<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseEntityCreatableController(ILogger<BaseEntityCreatableController<TEntity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <summary>
/// Controller providing read and create operations.
/// </summary>
/// <typeparam name="TEntity">The entity type implementing <see cref="IEntity"/> handled by this controller.</typeparam>
/// <typeparam name="TIdentity">The identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.CREATOR)]
public abstract class BaseEntityCreatableController<TEntity, TIdentity, TCriteria> : BaseEntityReadOnlyController<TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseEntityCreatableController(ILogger<BaseEntityCreatableController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Creates a single model instance.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created entity.</returns>
    /// <response code="201">Entity created.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.CREATE)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateAsync([FromBody][Required] TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityCreated = await this.Repository
            .AddAsync(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created(ActionRoutes.CREATE, entityCreated);
    }

    /// <summary>
    /// Creates a single model instance or if it already exist, retrieves it with included navigations.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created entity with included navigations.</returns>
    /// <response code="201">Entity created.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.CREATE_OR_GET)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateOrGetAsync([FromBody][Required] TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityCreatedOrGet = await this.Repository
            .AddOrGetAsync<TEntity, TIdentity>(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created(ActionRoutes.CREATE_OR_GET, entityCreatedOrGet);
    }

    /// <summary>
    /// Creates a single model instance and retrieves it with included navigations.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created entity with included navigations.</returns>
    /// <response code="201">Entity created.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.CREATE_AND_GET)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateAndGetAsync([FromBody][Required] TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityCreated = await this.Repository
            .AddAndGetAsync<TEntity, TIdentity>(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created(ActionRoutes.CREATE_AND_GET, entityCreated);
    }

    /// <summary>
    /// Creates multiple model instances.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="201">Entities created.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.CREATE_MANY)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .AddManyAsync(entities, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created();
    }

    /// <summary>
    /// Creates multiple model instances in bulk.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.CREATE_MANY_BULK)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateManyBulkAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .AddManyBulkAsync(entities, cancellationToken);

        return this.Created();
    }
}