using System;
using DynamicExpression.Interfaces;
using Microsoft.Extensions.Logging;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Entities;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public class DefaultIdentityController<TEntity, TCriteria> : BaseIdentityController<IRepository, TEntity, Guid, TCriteria>
    where TEntity : DefaultEntityUser, IEntityUpdatable, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected DefaultIdentityController(ILogger logger, IRepository repository, IIdentityRepository<Guid> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository, identityRepository, authExternalRepository)
    {
    }

    /// <inheritdoc />
    protected DefaultIdentityController(ILogger logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository, eventing, identityRepository, authExternalRepository)
    {
    }
}