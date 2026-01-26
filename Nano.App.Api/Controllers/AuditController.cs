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
/// 
/// </summary>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
public abstract class AuditController<TIdenity> : BaseControllerReadOnly<IRepository, AuditEntry<TIdenity>, TIdenity, AuditEntryQueryCriteria>
    where TIdenity : IEquatable<TIdenity>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="repository"></param>
    /// <param name="eventing"></param>
    protected AuditController(ILogger logger, IRepository repository, IEventing? eventing = null)
        : base(logger, repository, eventing)
    {
    }
}