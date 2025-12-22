using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using System;

namespace Nano.App.Web.Controllers;

/// <inheritdoc />
public abstract class BaseDefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IOptionsMonitor<WebOptions> options, IIdentityRepository<Guid> identityRepository, IAuthRepository<Guid> authRepository)
        : this(logger, repository, null, options, identityRepository, authRepository)
    {
    }

    /// <inheritdoc />
    protected BaseDefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IOptionsMonitor<WebOptions> options, IIdentityRepository<Guid> identityRepository, IAuthRepository<Guid> authRepository)
        : base(logger, repository, eventing, options, identityRepository, authRepository)
    {
    }
}