using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultControllerSpatialDeletable<TEntity, TCriteria> : DefaultControllerSpatialDeletable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityDeletable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerSpatialDeletable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialDeletable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerSpatialDeletable<TEntity, TIdentity, TCriteria> : BaseControllerSpatialDeletable<IRepositorySpatial, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityDeletable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerSpatialDeletable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialDeletable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}