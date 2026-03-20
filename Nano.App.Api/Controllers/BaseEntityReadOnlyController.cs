using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseEntityReadOnlyController<TEntity, TCriteria> : BaseEntityReadOnlyController<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseEntityReadOnlyController(ILogger<BaseEntityReadOnlyController<TEntity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <summary>
/// Controller providing read-only operations.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identifier.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
public abstract class BaseEntityReadOnlyController<TEntity, TIdentity, TCriteria> : BaseEntityViewController<TEntity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseEntityReadOnlyController(ILogger<BaseEntityReadOnlyController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Gets a single entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity matching the identifier.</returns>
    /// <response code="200">Entity retrieved successfully.</response>
    /// <response code="400">Invalid identifier.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">Entity not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.DETAILS)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEntity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsAsync([FromRoute][Required]TIdentity id, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        TEntity? result;

        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets multiple entities by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the entities.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entities matching the identifiers.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid identifiers.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.DETAILS_MANY)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<IEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsManyAsync([FromQuery][Required]TIdentity[] ids, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TIdentity>(ids, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);
        }

        return this.Ok(results);
    }

    /// <summary>
    /// Gets multiple entities by their identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the entities.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entities matching the identifiers.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid identifiers.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.DETAILS_MANY)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<IEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsManyPostAsync([FromBody][Required]TIdentity[] ids, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TIdentity>(ids, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);
        }

        return this.Ok(results);
    }
}