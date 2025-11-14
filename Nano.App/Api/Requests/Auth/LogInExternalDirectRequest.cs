using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalDirectRequest : BaseLogInExternalRequest<LogInExternalDirect>
{
    /// <inheritdoc />
    public LogInExternalDirectRequest()
    {
        this.Action = "login/external/direct";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}