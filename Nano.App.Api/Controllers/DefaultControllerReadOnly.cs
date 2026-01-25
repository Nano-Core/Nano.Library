using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public class DefaultControllerReadOnly<TEntity, TCriteria> : DefaultControllerReadOnly<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityIdentity<Guid>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultControllerReadOnly(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultControllerReadOnly<TEntity, TIdentity, TCriteria> : BaseControllerReadOnly<IRepository, TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityIdentity<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected DefaultControllerReadOnly(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}