using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultControllerSpatialWritable<TEntity, TCriteria> : DefaultControllerSpatialWritable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerSpatialWritable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialWritable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerSpatialWritable<TEntity, TIdentity, TCriteria> : BaseControllerSpatialWritable<IRepositorySpatial, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerSpatialWritable(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatialWritable(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}
