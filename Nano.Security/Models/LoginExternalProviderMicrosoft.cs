namespace Nano.Security.Models;

/// <summary>
/// LogIn External Provider Microsoft.
/// </summary>
public class LogInExternalProviderMicrosoft : LogInExternalProviderAuthCode
{
    /// <inheritdoc />
    public LogInExternalProviderMicrosoft()
    {
        this.Name = "Microsoft";
    }
}