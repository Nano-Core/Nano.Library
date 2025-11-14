using Nano.App.Consts;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <inheritdoc />
    public LogInExternalMicrosoftRequest()
    {
        this.Action = "login/external/microsoft";
        this.Controller = ControllerRoutes.AUTH_CONTROLLER_ROUTE;
    }
}