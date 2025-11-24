using Nano.App.ApiClient.Models.Identity.External;
using Nano.App.ApiClient.Requests.Auth;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <inheritdoc />
    public AddExternalLoginFacebookRequest()
    {
        this.Action = "external-logins/add/facebook";
    }
}