using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Mvc.Authorization.Consts;
using Nano.App.Consts;
using Nano.App.Extensions;

namespace Nano.App.Api.Controllers;

/// <summary>
/// Base API controller that provides common properties and behaviors for all derived controllers.
/// </summary>
[ApiController]
[Route("[controller]")]
[Route($"{ControllerRoutes.ROUTE_VERSION_PREFIX}/[controller]")]
[Authorize(Policy = AuthorizationPolicies.DEFAULT)]
public abstract class BaseController : Controller
{
    /// <summary>
    /// Logger instance for logging messages within the controller.
    /// </summary>
    protected virtual ILogger<BaseController> Logger { get; }

    /// <summary>
    /// Gets the current request identififer from header.
    /// </summary>
    public virtual string? RequestId => this.HttpContext.Request.GetRequestId();

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> used for logging within the controller.</param>
    protected BaseController(ILogger<BaseController> logger)
    {
        this.Logger = logger;
    }
}