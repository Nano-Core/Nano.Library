using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to add an external Facebook login.
/// </summary>
public class AddExternalLoginFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <summary>
    /// Initializes a new instance of <see cref="AddExternalLoginFacebookRequest"/>.
    /// Sets the action to "external-logins/add/facebook".
    /// </summary>
    public AddExternalLoginFacebookRequest()
    {
        this.Action = "external-logins/add/facebook";
    }
}