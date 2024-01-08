using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Const;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.READER)]
public abstract class BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria> : BaseController<TRepository>
    where TRepository : IRepository
    where TEntity : class, IEntityIdentity<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerReadOnly(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {

    }

    /// <inheritdoc />
    protected BaseControllerReadOnly(ILogger logger, TRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {

    }

    /// <summary>
    /// Gets all models.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>the models, matching the passed query.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("index")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IndexAsync([FromQuery][Required]IQuery query, CancellationToken cancellationToken = default)
    {
        query ??= new Query();

        var result = await this.Repository
            .GetManyAsync<TEntity>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets all models.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>the models, matching the passed query.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("index")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IndexPostAsync([FromBody][Required]IQuery query, CancellationToken cancellationToken = default)
    {
        query ??= new Query();

        var result = await this.Repository
            .GetManyAsync<TEntity>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <param name="id">The identifier, that uniquely identifies the model.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The model.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("details/{id}")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .GetAsync<TEntity, TIdentity>(id, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models.
    /// </summary>
    /// <param name="ids">The identifiers, that uniquely identifies the models.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("details/many")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsManyAsync([FromQuery][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the models.
    /// </summary>
    /// <param name="ids">The identifiers, that uniquely identifies the models.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("details/many")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DetailsManyPostAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Query models.
    /// </summary>
    /// <param name="query">The query model, containing filters used in the query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("query")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryAsync([FromQuery][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        query ??= new Query<TCriteria>();

        var result = await this.Repository
            .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Query models.
    /// </summary>
    /// <param name="query">The query model, containing filters used in the query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The models.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("query")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryPostAsync([FromBody][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        query ??= new Query<TCriteria>();

        var result = await this.Repository
            .GetManyAsync<TEntity, TCriteria>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Query the first mathcing entity.
    /// </summary>
    /// <param name="query">The query model, containing filters used in the query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The model.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("query/first")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryFirstAsync([FromQuery][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        query ??= new Query<TCriteria>();

        var result = await this.Repository
            .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Query the first mathcing entity.
    /// </summary>
    /// <param name="query">The query model, containing filters used in the query.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The model.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("query/first")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryFirstPostAsync([FromBody][Required]IQuery<TCriteria> query, CancellationToken cancellationToken = default)
    {
        query ??= new Query<TCriteria>();

        var result = await this.Repository
            .GetFirstAsync<TEntity, TCriteria>(query, cancellationToken);

        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the number of models (Count).
    /// </summary>
    /// <param name="criteria">The criteria model, containing filters used in the criteria.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The count of models..</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("query/count")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryCountAsync([FromQuery][Required]TCriteria criteria, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

        return this.Ok(result);
    }

    /// <summary>
    /// Gets the number of models (Count).
    /// </summary>
    /// <param name="criteria">The criteria model, containing filters used in the criteria.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The count of models..</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("query/count")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> QueryCountPostAsync([FromBody][Required]TCriteria criteria, CancellationToken cancellationToken = default)
    {
        var result = await this.Repository
            .CountAsync<TEntity, TCriteria>(criteria, cancellationToken);

        return this.Ok(result);
    }
}