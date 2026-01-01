using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Consts;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;

namespace Nano.App.Web.Controllers;

/// <inheritdoc />
[ControllerName(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, IOptionsMonitor<WebOptions> options, IAuthRepository authRepository, IAuthExternalRepository authExternalRepository, IAuthTransientRepository authTransientRepository)
        : base(logger, options, authRepository, authExternalRepository, authTransientRepository)
    {
    }
}