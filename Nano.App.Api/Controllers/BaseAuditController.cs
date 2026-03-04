using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Controllers.Criteria;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseAuditController(ILogger<BaseAuditController> logger, IRepository repository)
    : BaseAuditController<Guid>(logger, repository);

/// <summary>
/// Controller for audit-related operations.
/// </summary>
/// <param name="logger">The logger instance.</param>
/// <param name="repository">The repository instance.</param>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public abstract class BaseAuditController<TIdentity>(ILogger<BaseAuditController<TIdentity>> logger, IRepository repository)
    : BaseEntityReadOnlyController<IRepository, AuditEntry<TIdentity>, TIdentity, AuditEntryQueryCriteria>(logger, repository)
    where TIdentity : IEquatable<TIdentity>;