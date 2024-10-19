using Nano.App.Consts;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <inheritdoc />
    public LogInExternalGoogleRequest()
    {
        this.Action = "login/external/google";
        this.Controller = Constants.AUTH_CONTROLLER_ROUTE;
    }
}