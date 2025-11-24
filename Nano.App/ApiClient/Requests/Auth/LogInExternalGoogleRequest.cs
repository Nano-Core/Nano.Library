using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity.External;

namespace Nano.App.ApiClient.Requests.Auth;

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