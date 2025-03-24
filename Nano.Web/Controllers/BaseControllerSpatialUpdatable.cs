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
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.READER)]
public abstract class BaseControllerSpatialUpdatable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerSpatialReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepositorySpatial
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerSpatialUpdatable(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected BaseControllerSpatialUpdatable(ILogger logger, TRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.OK)]
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
    /// Edit the passed model, and reload.
    /// </summary>
    /// <param name="entity">The model to edit.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The edited model.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [HttpPost]
    [Route("edit/get")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditAndGetAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = await this.Repository
            .UpdateAndGetAsync<TEntity, TIdentity>(entity, cancellationToken);

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .UpdateManyAsync(entities, cancellationToken);

        if (entities == null)
        {
            return this.NotFound();
        }

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }
}