using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity.External;

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