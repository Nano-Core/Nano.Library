using System;
using Asp.Versioning;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Security;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[ControllerName(Constants.AUTH_CONTROLLER_ROUTE)]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, DefaultIdentityManager baseIdentityManager)
        : base(logger, baseIdentityManager)
    {
    }
}