using Nano.App.ApiClient.Consts;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

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