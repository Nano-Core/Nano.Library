using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an external Facebook account.
/// </summary>
public class LogInExternalFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogInExternalFacebookRequest"/> with action and controller set.
    /// </summary>
    public LogInExternalFacebookRequest()
    {
        this.Action = "login/external/facebook";
    }
}