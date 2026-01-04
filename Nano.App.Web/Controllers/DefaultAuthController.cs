using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.ApiClient.Consts;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Web.Controllers;

/// <inheritdoc />
[ControllerName(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, IIdentityAuthRepository identityAuthRepository, IAuthRootRepository authRootRepository, IAuthExternalRepository authExternalRepository)
        : base(logger, identityAuthRepository, authRootRepository, authExternalRepository)
    {
    }
}