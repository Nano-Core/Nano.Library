using System;
using Asp.Versioning;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Security;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[ControllerName(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, IIdentityAuthRepository<Guid> identityRepository, IIdentityAuthTransientRepository<Guid> identityTransientRepository)
        : base(logger, identityRepository, identityTransientRepository)
    {
    }
}