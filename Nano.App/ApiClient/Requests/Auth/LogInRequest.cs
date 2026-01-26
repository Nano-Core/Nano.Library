using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in a standard user.
/// </summary>
public class LogInRequest : BaseRequestPost
{
    /// <summary>
    /// Contains the login details for standard login.
    /// </summary>
    public virtual LogIn LogIn { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="LogInRequest"/> with action and controller set.
    /// </summary>
    public LogInRequest()
    {
        this.Action = "login";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the request containing the login details.
    /// </summary>
    public override object GetBody()
    {
        return this.LogIn;
    }
}