using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <summary>
/// Controller providing read-only operations.
/// </summary>
/// <typeparam name="TRepository">The repository implementing <see cref="IRepository"/> used by this controller.</typeparam>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TIdentity">The type of the entity's identifier.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.CREATOR + "," + BuiltInUserRoles.EDITOR + "," + BuiltInUserRoles.DELETER + "," + BuiltInUserRoles.READER)]
public abstract class BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria> : BaseController<TRepository>
    where TRepository : class, IRepository
    where TEntity : class, IEntityIdentity<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="repository">The repository instance.</param>
    /// <param name="eventing">Optional eventing service.</param>
    protected BaseControllerReadOnly(ILogger logger, TRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Gets all entities matching the specified query.
    /// </summary>
    /// <param name="query">The query used to filter entities.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities matching the query.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.INDEX)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IndexAsync([FromQuery][Required]IQuery query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity>(query, cancellationToken);
        }

        return this.Ok(results);
    }

    /// <summary>
    /// Gets all entities matching the specified query via POST.
    /// </summary>
    /// <param name="query">The query used to filter entities.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities matching the query.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.INDEX)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IndexPostAsync([FromBody][Required]IQuery query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity>(query, cancellationToken);
        }

        return this.Ok(results);
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
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.OK)]
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
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
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
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
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

    /// <summary>
    /// Queries entities matching the specified <typeparamref name="TCriteria"/>.
    /// </summary>
    /// <param name="query">The query model containing filters and criteria.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities matching the criteria.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.QUERY)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryAsync([FromQuery][Required]IQuery<TCriteria> query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);
        }

        return this.Ok(results);
    }

    /// <summary>
    /// Queries entities matching the specified <typeparamref name="TCriteria"/> via POST.
    /// </summary>
    /// <param name="query">The query model containing filters and criteria.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities matching the criteria.</returns>
    /// <response code="200">Entities retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.QUERY)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryPostAsync([FromBody][Required]IQuery<TCriteria> query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> results;

        if (includeDepth.HasValue)
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            results = await this.Repository
                .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);
        }

        return this.Ok(results);
    }


    /// <summary>
    /// Retrieves the first entity matching the specified <typeparamref name="TCriteria"/>.
    /// </summary>
    /// <param name="query">The query model containing filters and criteria.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first entity matching the criteria.</returns>
    /// <response code="200">Entity retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entity found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.QUERY_FIRST)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryFirstAsync([FromQuery][Required]IQuery<TCriteria> query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        TEntity? result;

        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Retrieves the first entity matching the specified <typeparamref name="TCriteria"/> via POST.
    /// </summary>
    /// <param name="query">The query model containing filters and criteria.</param>
    /// <param name="includeDepth">Optional include depth for related entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first entity matching the criteria.</returns>
    /// <response code="200">Entity retrieved successfully.</response>
    /// <response code="400">Invalid query parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entity found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.QUERY_FIRST)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryFirstPostAsync([FromBody][Required]IQuery<TCriteria> query, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        TEntity? result;

        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the total count of entities matching the specified <typeparamref name="TCriteria"/>.
    /// </summary>
    /// <param name="criteria">The criteria model containing filters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities matching the criteria.</returns>
    /// <response code="200">Count retrieved successfully.</response>
    /// <response code="400">Invalid criteria parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.QUERY_COUNT)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryCountAsync([FromQuery][Required]TCriteria criteria, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the total count of entities matching the specified <typeparamref name="TCriteria"/> via POST.
    /// </summary>
    /// <param name="criteria">The criteria model containing filters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities matching the criteria.</returns>
    /// <response code="200">Count retrieved successfully.</response>
    /// <response code="400">Invalid criteria parameters.</response>
    /// <response code="401">Unauthorized access.</response>
    /// <response code="404">No entities found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.QUERY_COUNT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryCountPostAsync([FromBody][Required]TCriteria criteria, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

        return this.Ok(result);
    }
}