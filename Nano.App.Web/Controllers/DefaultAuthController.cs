using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.ApiClient.Consts;
using Nano.App.Web.Identity.Abstractions;

namespace Nano.App.Web.Controllers;

/// <inheritdoc />
[ControllerName(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, IAuthRepository authRepository, IAuthTransientRepository authTransientRepository)
        : base(logger, authRepository, authTransientRepository)
    {
    }
}