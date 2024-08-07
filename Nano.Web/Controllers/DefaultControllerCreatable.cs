using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Models.Eventing.Interfaces;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultControllerCreatable<TEntity, TCriteria> : DefaultControllerCreatable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerCreatable(ILogger logger, IRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerCreatable(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerCreatable<TEntity, TIdentity, TCriteria> : BaseControllerCreatable<IRepository, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityCreatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerCreatable(ILogger logger, IRepository repository)
        : this(logger, repository, new NullEventing())
    {
    }

    /// <inheritdoc />
    protected DefaultControllerCreatable(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}