using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Controllers.Criteria;
using Nano.App.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models;
using System;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseAuditController(ILogger<BaseAuditController> logger, IRepository repository)
    : BaseAuditController<Guid>(logger, repository);

/// <summary>
/// Controller for audit-related operations.
/// </summary>
/// <param name="logger">The logger instance.</param>
/// <param name="repository">The repository instance.</param>
[Route(ControllerRoutes.AUDIT)]
[Route($"{ControllerRoutes.ROUTE_VERSION_PREFIX}/{ControllerRoutes.AUDIT}")]
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public abstract class BaseAuditController<TIdentity>(ILogger<BaseAuditController<TIdentity>> logger, IRepository repository)
    : BaseEntityReadOnlyController<AuditEntry<TIdentity>, TIdentity, AuditEntryQueryCriteria<TIdentity>>(logger, repository)
    where TIdentity : IEquatable<TIdentity>;