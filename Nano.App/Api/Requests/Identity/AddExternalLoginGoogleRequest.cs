using Nano.App.Api.Requests.Auth;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginGoogleRequest : BaseLogInExternalRequest<LogInExternalGoogle>
{
    /// <inheritdoc />
    public AddExternalLoginGoogleRequest()
    {
        this.Action = "external-logins/add/google";
    }
}