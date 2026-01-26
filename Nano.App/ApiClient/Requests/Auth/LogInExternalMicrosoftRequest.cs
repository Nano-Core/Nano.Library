using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an external Microsoft account.
/// </summary>
public class LogInExternalMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogInExternalMicrosoftRequest"/> with action and controller set.
    /// </summary>
    public LogInExternalMicrosoftRequest()
    {
        this.Action = "login/external/microsoft";
    }
}