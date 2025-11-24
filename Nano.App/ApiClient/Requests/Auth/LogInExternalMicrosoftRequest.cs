using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity.External;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class LogInExternalMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <inheritdoc />
    public LogInExternalMicrosoftRequest()
    {
        this.Action = "login/external/microsoft";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}