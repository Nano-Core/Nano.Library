namespace Nano.App.ApiClient.Models.Identity.External.Providers;

/// <summary>
/// External Login Provider Facebook.
/// </summary>
public class ExternalLoginProviderFacebook : ExternalLoginProviderImplicit
{
    /// <inheritdoc />
    public ExternalLoginProviderFacebook()
    {
        this.Name = "Facebook";
    }
}