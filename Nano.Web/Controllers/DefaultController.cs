using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public class DefaultController<TEntity, TCriteria> : DefaultController<TEntity, Guid, TCriteria>
    where TEntity : DefaultEntity, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultController(ILogger logger, IRepository repository)
        : this(logger, repository, new NullEventing())
    {

    }

    /// <inheritdoc />
    protected DefaultController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {

    }
}

/// <inheritdoc />
public class DefaultController<TEntity, TIdentity, TCriteria> : BaseControllerWritable<IRepository, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityWritable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultController(ILogger logger, IRepository repository)
        : this(logger, repository, new NullEventing())
    {

    }

    /// <inheritdoc />
    protected DefaultController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {

    }
}