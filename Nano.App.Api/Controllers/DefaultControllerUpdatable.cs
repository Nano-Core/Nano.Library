using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public class DefaultControllerUpdatable<TEntity, TCriteria> : DefaultControllerUpdatable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerUpdatable(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerUpdatable<TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<IRepository, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityUpdatable
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerUpdatable(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}