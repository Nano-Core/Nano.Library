using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.App.Web.Controllers;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using System;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public abstract class BaseDefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IIdentityRepository<Guid> identityRepository, IIdentityAuthRepository<Guid> identityAuthRepository)
        : this(logger, repository, null, identityRepository, identityAuthRepository)
    {
    }

    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> identityRepository, IIdentityAuthRepository<Guid> identityAuthRepository)
        : base(logger, repository, eventing, identityRepository, identityAuthRepository)
    {
    }
}