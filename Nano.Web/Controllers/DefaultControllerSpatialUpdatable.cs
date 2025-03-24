using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultControllerSpatialUpdatable<TEntity, TCriteria> : DefaultControllerSpatialUpdatable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerSpatialUpdatable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialUpdatable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerSpatialUpdatable<TEntity, TIdentity, TCriteria> : BaseControllerSpatialUpdatable<IRepositorySpatial, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerSpatialUpdatable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialUpdatable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}