using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Auth;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in as a root user.
/// </summary>
public class LogInRootRequest : BaseRequestPost
{
    /// <summary>
    /// Contains the login details for root login.
    /// </summary>
    public virtual LogInRoot LogInRoot { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="LogInRootRequest"/> with action and controller set.
    /// </summary>
    public LogInRootRequest()
    {
        this.Action = "login/root";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the request containing the root login details.
    /// </summary>
    public override object GetBody()
    {
        return this.LogInRoot;
    }
}