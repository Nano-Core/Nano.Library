using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalDirectRequest : BaseLogInExternalRequest<LogInExternalDirect>
{
    /// <inheritdoc />
    public LogInExternalDirectRequest()
    {
        this.Action = "login/external/direct";
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }
}