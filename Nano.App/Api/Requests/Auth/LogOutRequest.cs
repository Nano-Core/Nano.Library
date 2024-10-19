using Nano.App.Consts;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogOutRequest : BaseRequestPost
{
    /// <inheritdoc />
    public LogOutRequest()
    {
        this.Action = "logout";
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}