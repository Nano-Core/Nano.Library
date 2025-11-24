using Nano.App.ApiClient.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class LogOutRequest : BaseRequestPost
{
    /// <inheritdoc />
    public LogOutRequest()
    {
        this.Action = "logout";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}