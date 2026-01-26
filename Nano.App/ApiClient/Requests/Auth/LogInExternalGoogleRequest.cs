using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an external Google account.
/// </summary>
public class LogInExternalGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogInExternalGoogleRequest"/> with action and controller set.
    /// </summary>
    public LogInExternalGoogleRequest()
    {
        this.Action = "login/external/google";
    }
}