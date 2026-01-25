using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an external refresh login request using Google authentication.
/// </summary>
public class LogInExternalRefreshGoogle : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshGoogle()
    {
        this.ProviderName = ExternalLogInProviderNames.GOOGLE;
    }
}
