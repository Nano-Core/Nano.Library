namespace Nano.Data.Abstractions.Identity.Models;

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