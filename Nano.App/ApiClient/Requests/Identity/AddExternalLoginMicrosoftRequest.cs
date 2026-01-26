using Nano.App.ApiClient.Requests.Auth;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Request to add an external Microsoft login.
/// </summary>
public class AddExternalLoginMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <summary>
    /// Initializes a new instance of <see cref="AddExternalLoginMicrosoftRequest"/>.
    /// Sets the action to "external-logins/add/microsoft".
    /// </summary>
    public AddExternalLoginMicrosoftRequest()
    {
        this.Action = "external-logins/add/microsoft";
    }
}