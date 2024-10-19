using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

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
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.LogIn;
    }
}