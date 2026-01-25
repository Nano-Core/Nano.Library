using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an external refresh login request using Microsoft authentication.
/// </summary>
public class LogInExternalRefreshMicrosoft : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshMicrosoft()
    {
        this.ProviderName = ExternalLogInProviderNames.MICROSOFT;
    }
}