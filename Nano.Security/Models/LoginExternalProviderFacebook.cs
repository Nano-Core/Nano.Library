namespace Nano.Security.Models;

/// <summary>
/// LogIn External Provider Facebook.
/// </summary>
public class LogInExternalProviderFacebook : LogInExternalProviderImplicit
{
    /// <inheritdoc />
    public LogInExternalProviderFacebook()
    {
        this.Name = "Facebook";
    }
}