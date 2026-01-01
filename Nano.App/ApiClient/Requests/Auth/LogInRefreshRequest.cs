using Nano.App.ApiClient.Consts;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class LogInRefreshRequest : BaseRequestPost
{
    /// <summary>
    /// Login.
    /// </summary>
    public virtual LogInRefresh LogInRefresh { get; set; } = new();

    /// <inheritdoc />
    public LogInRefreshRequest()
    {
        this.Action = "login/refresh";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LogInRefresh;
    }
}