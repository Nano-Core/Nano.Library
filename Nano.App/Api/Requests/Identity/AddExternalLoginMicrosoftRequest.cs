using Nano.App.Api.Requests.Auth;
using Nano.Security.Models;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class AddExternalLoginMicrosoftRequest : BaseLogInExternalRequest<LogInExternalMicrosoft>
{
    /// <inheritdoc />
    public AddExternalLoginMicrosoftRequest()
    {
        this.Action = "external-logins/add/microsoft";
    }
}