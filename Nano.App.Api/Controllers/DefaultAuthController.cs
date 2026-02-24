using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Identity.Authentication.Abstractions;
using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
[ControllerName(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger<DefaultAuthController> logger, IIdentityAuthRepository? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, identityAuthRepository, authTransientRepository, authRootRepository, authExternalRepository)
    {
    }
}