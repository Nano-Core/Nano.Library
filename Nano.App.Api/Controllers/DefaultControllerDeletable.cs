using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public class DefaultControllerDeletable<TEntity, TCriteria> : DefaultControllerDeletable<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>, IEntityDeletable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerDeletable<TEntity, TIdentity, TCriteria> : BaseControllerDeletable<IRepository, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable, new()
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerDeletable(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}