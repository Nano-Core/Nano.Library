using Nano.App.ApiClient.Requests.Auth;
using Nano.Common.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <inheritdoc />
    public AddExternalLoginMicrosoftRequest()
    {
        this.Action = "external-logins/add/microsoft";
    }
}