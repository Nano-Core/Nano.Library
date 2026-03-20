using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.ApiClient.Requests.Models;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions;
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
public abstract class BaseEntityUpdatableController<TEntity, TCriteria> : BaseEntityUpdatableController<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseEntityUpdatableController(ILogger<BaseEntityUpdatableController<TEntity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <summary>
/// Controller providing update operations.
/// </summary>
/// <typeparam name="TEntity">The entity type implementing <see cref="IEntity"/> handled by this controller.</typeparam>
/// <typeparam name="TIdentity">The identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
public abstract class BaseEntityUpdatableController<TEntity, TIdentity, TCriteria> : BaseEntityReadOnlyController<TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseEntityUpdatableController(ILogger<BaseEntityUpdatableController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Edits a single model instance.
    /// </summary>
    /// <param name="entity">The entity to edit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The edited entity.</returns>
    /// <response code="200">Entity updated.</response>
    /// <response code="404">Entity not found.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityEdited = await this.Repository
            .UpdateAsync(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok(entityEdited);
    }

    /// <summary>
    /// Edits a single model instance and retrieves it with included navigations.
    /// </summary>
    /// <param name="entity">The entity to edit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The edited entity.</returns>
    /// <response code="201">Entity updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT_GET)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditAndGetAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        var entityEdited = await this.Repository
            .UpdateAndGetAsync<TEntity, TIdentity>(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok(entityEdited);
    }

    /// <summary>
    /// Edits multiple model instances.
    /// </summary>
    /// <param name="entities">The entities to edit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT_MANY)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .UpdateManyAsync(entities, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Edits multiple model instances in bulk.
    /// </summary>
    /// <param name="entities">The entities to edit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT_MANY_BULK)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditManyBulkAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .UpdateManyBulkAsync(entities, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Edits entities that match the specified criteria.
    /// </summary>
    /// <param name="query">The update query containing criteria and property updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT_QUERY)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditQueryAsync([FromBody][Required]UpdateQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .UpdateManyAsync<TEntity, TCriteria>(query.Criteria, query.PropertyUpdates, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Edits entities that match the specified criteria in bulk.
    /// </summary>
    /// <param name="query">The update query containing criteria and property updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Entities updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut]
    [HttpPost]
    [Route(ActionRoutes.EDIT_QUERY_BULK)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditQueryBulkAsync([FromBody][Required] UpdateQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .UpdateManyBulkAsync<TEntity, TCriteria>(query.Criteria, query.PropertyUpdates, cancellationToken);

        return this.Ok();
    }
}