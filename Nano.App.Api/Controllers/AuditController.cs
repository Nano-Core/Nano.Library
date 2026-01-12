using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Controllers.Models;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models;
using Nano.Eventing.Abstractions;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public class AuditController : DefaultControllerReadOnly<DefaultAuditEntry, AuditEntryQueryCriteria>
{
    /// <inheritdoc />
    public AuditController(ILogger logger, IRepository repository, IEventing eventing)
        : base(logger, repository, eventing)
    {
    }
}