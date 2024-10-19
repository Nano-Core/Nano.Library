using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

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
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LogInRefresh;
    }
}