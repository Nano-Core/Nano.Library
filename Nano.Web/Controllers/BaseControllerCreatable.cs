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
using Nano.Models.Data;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security.Const;

namespace Nano.Web.Controllers;

/// <summary>
/// Base abstract <see cref="Controller"/>, implementing  methods for instances of <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TRepository">The <see cref="IRepository"/> inheriting from <see cref="BaseControllerReadOnly{TRepository,TEntity,TIdentity,TCriteria}"/>.</typeparam>
/// <typeparam name="TEntity">The <see cref="IEntity"/> model the <see cref="IRepository"/> operates with.</typeparam>
/// <typeparam name="TIdentity">The Identifier type of <typeparamref name="TEntity"/>.</typeparam>
/// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> implementation.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER)]
public abstract class BaseControllerCreatable<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepository
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseControllerCreatable(ILogger logger, TRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected BaseControllerCreatable(ILogger logger, TRepository repository, IEventing eventing)
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
    public virtual async Task<IActionResult> CreateAndGetAsync([FromBody][Required] TEntity entity, CancellationToken cancellationToken = default)
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
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create/many")]
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

    /// <summary>
    /// Creates the passed models bulk.
    /// </summary>
    /// <param name="entities">The models to create.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("create/many/bulk")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateManyBulkAsync([FromBody][Required]IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await this.Repository
            .AddManyBulkAsync(entities, cancellationToken);

        return this.Created();
    }
}