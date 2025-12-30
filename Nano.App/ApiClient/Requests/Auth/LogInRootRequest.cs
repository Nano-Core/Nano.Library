using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class LogInRootRequest : BaseRequestPost
{
    /// <summary>
    /// Log In Root.
    /// </summary>
    public virtual LogInRoot LogInRoot { get; set; } = new();

    /// <inheritdoc />
    public LogInRootRequest()
    {
        this.Action = "login/root";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LogInRoot;
    }
}