using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Provider Facebook.
/// </summary>
public class ExternalLoginProviderFacebook : ExternalLoginProviderImplicit
{
    /// <inheritdoc />
    public ExternalLoginProviderFacebook()
    {
        this.Name = ExternalLogInProviderNames.FACEBOOK;
    }
}