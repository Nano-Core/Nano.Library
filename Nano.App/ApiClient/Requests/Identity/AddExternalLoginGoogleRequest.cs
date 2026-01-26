using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to add an external Google login.
/// </summary>
public class AddExternalLoginGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <summary>
    /// Initializes a new instance of <see cref="AddExternalLoginGoogleRequest"/>.
    /// Sets the action to "external-logins/add/google".
    /// </summary>
    public AddExternalLoginGoogleRequest()
    {
        this.Action = "external-logins/add/google";
    }
}