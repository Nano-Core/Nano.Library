using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Controllers.Models;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <summary>
/// Controller for audit-related operations.
/// </summary>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public abstract class AuditController<TIdentity> : BaseControllerReadOnly<IRepository, AuditEntry<TIdentity>, TIdentity, AuditEntryQueryCriteria>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditController{TIdentity}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="repository">The repository instance.</param>
    /// <param name="eventing">Optional eventing service.</param>
    protected AuditController(ILogger<AuditController<TIdentity>> logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}