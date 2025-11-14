using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <inheritdoc />
    public LogInExternalGoogleRequest()
    {
        this.Action = "login/external/google";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}