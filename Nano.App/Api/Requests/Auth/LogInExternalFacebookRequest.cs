using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <inheritdoc />
    public LogInExternalFacebookRequest()
    {
        this.Action = "login/external/facebook";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}