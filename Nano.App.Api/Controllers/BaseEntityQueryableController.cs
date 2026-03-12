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

/// <summary>
/// Controller providing view index and query operations.
/// </summary>
/// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
/// <typeparam name="TCriteria">The query criteria type implementing <see cref="IQueryCriteria"/>.</typeparam>
public abstract class BaseEntityQueryableController<TEntity, TCriteria> : BaseEntityController
    where TEntity : class, IEntity
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseEntityQueryableController(ILogger<BaseEntityQueryableController<TEntity, TCriteria>> logger, IRepository repository, IEventing? eventing = null)
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
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