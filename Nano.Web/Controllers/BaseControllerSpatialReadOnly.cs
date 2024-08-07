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
using Nano.Eventing;
using Nano.Models;
using Nano.Models.Const;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.READER)]
public abstract class BaseControllerSpatialReadOnly<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepositorySpatial
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial
    where TCriteria : class, IQueryCriteriaSpatial, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerSpatialReadOnly(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected BaseControllerSpatialReadOnly(ILogger logger, TRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }

    /// <summary>
    /// Gets the models, that intersects.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("intersects")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IntersectsAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Intersects<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Intersects<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that covers.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("covers")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CoversAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Covers<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Covers<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that are covered-by.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("covered-by")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CoveredByAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .CoveredBy<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .CoveredBy<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that are covered-by.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("overlaps")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> OverlapsAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Overlaps<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Overlaps<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that tocuhes.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("touches")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> TouchesAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Touches<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Touches<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that crosses.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("crosses")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CrossesAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Crosses<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Crosses<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that are disjointed.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("disjoints")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DisjointsAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Disjoints<TEntity, TCriteria>(criteria, includeDepth.Value, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Disjoints<TEntity, TCriteria>(criteria, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models, that are within.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="distance">The distance in meters. Default: 50.000 meters.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("within")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<DefaultEntity>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> WithinAsync([FromBody][Required]IQuery<TCriteria> criteria, [FromQuery]int? includeDepth, [FromQuery]double distance = 50000, CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> result;
        if (includeDepth.HasValue)
        {
            result = await this.Repository
                .Within<TEntity, TCriteria>(criteria, includeDepth.Value, distance, cancellationToken);
        }
        else
        {
            result = await this.Repository
                .Within<TEntity, TCriteria>(criteria, distance, cancellationToken);
        }

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }
}