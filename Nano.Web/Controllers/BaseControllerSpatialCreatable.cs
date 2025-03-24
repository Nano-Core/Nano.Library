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
public abstract class BaseControllerSpatialCreatable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerSpatialReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepositorySpatial
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerSpatialCreatable(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected BaseControllerSpatialCreatable(ILogger logger, TRepository repository, IEventing eventing)
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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.Created)]
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
    /// Create the passed model, and reload.
    /// </summary>
    /// <param name="entity">The model to create.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The created model.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create/get")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntity), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateAndGetAsync([FromBody][Required]TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = await this.Repository
            .AddAndGetAsync<TEntity, TIdentity>(entity, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created("create/get", entity);
    }

    /// <summary>
    /// Creates the passed models.
    /// </summary>
    /// <param name="entities">The models to create.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The created models.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create/Many")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateManyAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .AddManyAsync(entities, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Created();
    }
}