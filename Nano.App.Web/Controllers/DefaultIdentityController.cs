using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using System;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;

namespace Nano.App.Web.Controllers;

/// <inheritdoc />
public class DefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultIdentityController(ILogger logger, IRepository repository, IIdentityRepository<Guid> identityRepository, IAuthExternalRepository authExternalRepository = null)
        : this(logger, repository, null, identityRepository, authExternalRepository)
    {
    }

    /// <inheritdoc />
    protected DefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> identityRepository, IAuthExternalRepository authExternalRepository = null)
        : base(logger, repository, eventing, identityRepository, authExternalRepository)
    {
    }
}