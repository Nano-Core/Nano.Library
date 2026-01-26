using Nano.App.ApiClient.Consts;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log out the current user.
/// </summary>
public class LogOutRequest : BaseRequestPost
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogOutRequest"/> and sets the action and controller.
    /// </summary>
    public LogOutRequest()
    {
        this.Action = "logout";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the logout request, which is always null.
    /// </summary>
    public override object? GetBody()
    {
        return null;
    }
}