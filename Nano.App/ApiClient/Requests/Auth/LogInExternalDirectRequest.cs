using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Represents a request to log in using an external direct account.
/// </summary>
public class LogInExternalDirectRequest : BaseLogInExternalRequest<LogInExternalDirect>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LogInExternalDirectRequest"/> with action and controller set.
    /// </summary>
    public LogInExternalDirectRequest()
    {
        this.Action = "login/external/direct";
    }
}