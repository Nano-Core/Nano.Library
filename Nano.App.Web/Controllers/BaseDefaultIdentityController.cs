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
using Nano.Data.Abstractions.Identity.Abstractions;

namespace Nano.Web.Controllers;

/// <inheritdoc />
public abstract class BaseDefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IIdentityRepository<Guid> identityRepository, IAuthRepository<Guid> authRepository)
        : this(logger, repository, null, identityRepository, authRepository)
    {
    }

    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> identityRepository, IAuthRepository<Guid> authRepository)
        : base(logger, repository, eventing, identityRepository, authRepository)
    {
    }
}