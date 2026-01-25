using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an external refresh login request using Facebook authentication.
/// </summary>
public class LogInExternalRefreshFacebook : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshFacebook()
    {
        this.ProviderName = ExternalLogInProviderNames.FACEBOOK;
    }
}