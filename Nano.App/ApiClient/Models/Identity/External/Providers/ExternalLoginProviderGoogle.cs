namespace Nano.App.ApiClient.Models.Identity.External.Providers;

/// <summary>
/// External Login Provider Google.
/// </summary>
public class ExternalLoginProviderGoogle : ExternalLoginProviderImplicit
{
    /// <inheritdoc />
    public ExternalLoginProviderGoogle()
    {
        this.Name = "Google";
    }
}