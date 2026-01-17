using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// 
/// </summary>
public class LogInExternalRefreshFacebook : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshFacebook()
    {
        this.ProviderName = ExternalLogInProviderNames.FACEBOOK;
    }
}