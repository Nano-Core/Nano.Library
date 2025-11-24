using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Requests.Auth;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <inheritdoc />
    public AddExternalLoginGoogleRequest()
    {
        this.Action = "external-logins/add/google";
    }
}