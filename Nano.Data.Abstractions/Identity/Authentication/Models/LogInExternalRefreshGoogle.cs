using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// 
/// </summary>
public class LogInExternalRefreshGoogle : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshGoogle()
    {
        this.ProviderName = ExternalLogInProviderNames.GOOGLE;
    }
}