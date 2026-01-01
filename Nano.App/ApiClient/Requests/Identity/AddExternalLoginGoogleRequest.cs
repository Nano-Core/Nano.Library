using Nano.App.ApiClient.Requests.Auth;
using Nano.Common.Identity.Authentication.Models;

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