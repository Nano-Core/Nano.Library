using System;
using Asp.Versioning;
using Microsoft.Extensions.Logging;
using Nano.Security;

namespace Nano.Web.Controllers;

/// <inheritdoc />
[ControllerName("Auth")]
public class DefaultAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public DefaultAuthController(ILogger logger, DefaultIdentityManager baseIdentityManager)
        : base(logger, baseIdentityManager)
    {
    }
}