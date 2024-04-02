using System;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Models.Criterias.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultControllerSpatial<TEntity, TCriteria> : DefaultControllerSpatial<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial, IEntityWritable, new()
    where TCriteria : class, IQueryCriteriaSpatial, new()
{
    /// <inheritdoc />
    protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerSpatial<TEntity, TIdentity, TCriteria> : BaseControllerSpatialWritable<IRepositorySpatial, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntitySpatial, IEntityWritable, new()
    where TCriteria : class, IQueryCriteriaSpatial, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerSpatial(ILogger logger, IRepositorySpatial repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}