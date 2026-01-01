using Nano.App.ApiClient.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <inheritdoc />
public class LogInRequest : BaseRequestPost
{
    /// <summary>
    /// LogIn.
    /// </summary>
    public virtual LogIn LogIn { get; set; } = new();

    /// <inheritdoc />
    public LogInRequest()
    {
        this.Action = "login";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LogIn;
    }
}