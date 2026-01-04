namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// 
/// </summary>
public class LogInExternalRefreshMicrosoft : LogInExternalRefresh
{
    /// <inheritdoc />
    public LogInExternalRefreshMicrosoft()
    {
        this.ProviderName = "Microsoft";
    }
}