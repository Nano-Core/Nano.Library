using Nano.App.Api.Requests.Auth;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginFacebookRequest : BaseLogInExternalRequest<LogInExternalFacebook>
{
    /// <inheritdoc />
    public AddExternalLoginFacebookRequest()
    {
        this.Action = "external-logins/add/facebook";
    }
}