using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing;
using Nano.Eventing.Abstractions;
using Nano.Security;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public abstract class BaseDefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IIdentityRepository<Guid> baseIdentityManager)
        : this(logger, repository, null, baseIdentityManager)
    {
    }

    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> baseIdentityManager)
        : base(logger, repository, eventing, baseIdentityManager)
    {
    }
}