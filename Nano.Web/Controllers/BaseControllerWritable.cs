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
using Nano.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;
using Nano.Web.Const;
using Nano.Web.Models;

namespace Nano.Web.Controllers;

/// <summary>
/// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TRepository">The <see cref="IRepository"/> inheriting from <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/>.</typeparam>
/// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IRepository"/> operates with.</typeparam>
/// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER)]
public abstract class BaseControllerWritable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepository
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerWritable(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {

    }

    /// <inheritdoc />
    protected BaseControllerWritable(ILogger logger, TRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {

    }

    /// <summary>
    /// Create the passed model.
    /// </summary>
    /// <param name="entity">The model to create.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The created model.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = await this.Repository
            .AddAsync(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created("create", entity);
    }

    /// <summary>
    /// Creates the passed models.
    /// </summary>
    /// <param name="entities">The models to create.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The created models.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create/Many")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(IEnumerable<object>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        entities = await this.Repository
            .AddManyAsync(entities, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created("create/many", entities);
    }

    /// <summary>
    /// Edit the passed model.
    /// </summary>
    /// <param name="entity">The model to edit.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The edited model.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [HttpPost]
    [Route("edit")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = await this.Repository
            .UpdateAsync(entity, cancellationToken);

        if (entity == null)
        {
            return this.NotFound();
        }

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok(entity);
    }

    /// <summary>
    /// Edits the passed models.
    /// </summary>
    /// <param name="entities">The models to edit.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The edited models.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [HttpPost]
    [Route("edit/many")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        entities = await this.Repository
            .UpdateManyAsync(entities, cancellationToken);

        if (entities == null)
        {
            return this.NotFound();
        }

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok(entities);
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
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
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