using Nano.Security.Models;

namespace Nano.App.Api.Requests.Auth;

/// <inheritdoc />
public class LogInExternalFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <inheritdoc />
    public LogInExternalFacebookRequest()
    {
        this.Action = "login/external/facebook";
        this.Controller = "auth";
    }
}