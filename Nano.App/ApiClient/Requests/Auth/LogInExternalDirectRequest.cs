using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity.External;

namespace Nano.App.ApiClient.Requests.Auth;

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