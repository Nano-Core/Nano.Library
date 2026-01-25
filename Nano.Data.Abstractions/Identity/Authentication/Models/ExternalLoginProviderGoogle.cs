using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login provider using Google authentication.
/// </summary>
public class ExternalLoginProviderGoogle : ExternalLoginProviderImplicit
{
    /// <inheritdoc />
    public ExternalLoginProviderGoogle()
    {
        this.Name = ExternalLogInProviderNames.GOOGLE;
    }
}