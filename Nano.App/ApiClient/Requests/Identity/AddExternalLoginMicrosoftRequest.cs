using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Requests.Auth;

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