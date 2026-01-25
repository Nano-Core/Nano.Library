using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Controllers.Models;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Entities;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

// BUG: Should have base for choice of TIdenty matches.
// BUG: Also look into the auditable interfacs vs annotations. Try and get rid of the interfaces.
// BUG: Web Application, Derived from Api Application (check web application root, and services and appbuilder extensions)

/// <summary>
/// 
/// </summary>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public abstract class BaseAuditController<TIdenity> : BaseControllerReadOnly<IRepository, DefaultAuditEntry, TIdenity, AuditEntryQueryCriteria>
    where TIdenity : IEquatable<TIdenity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="repository"></param>
    /// <param name="eventing"></param>
    protected BaseAuditController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}

/// <inheritdoc />
public class DefaultAuditController : DefaultControllerReadOnly<DefaultAuditEntry, AuditEntryQueryCriteria>
{
    /// <inheritdoc />
    public DefaultAuditController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}