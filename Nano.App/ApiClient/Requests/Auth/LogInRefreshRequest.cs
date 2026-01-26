using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to refresh an existing login session.
/// </summary>
public class LogInRefreshRequest : BaseRequestPost
{
    /// <summary>
    /// Contains the refresh login details.
    /// </summary>
    public virtual LogInRefresh LogInRefresh { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="LogInRefreshRequest"/> with action and controller set.
    /// </summary>
    public LogInRefreshRequest()
    {
        this.Action = "login/refresh";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <summary>
    /// Gets the body of the request containing the refresh login details.
    /// </summary>
    public override object GetBody()
    {
        return this.LogInRefresh;
    }
}