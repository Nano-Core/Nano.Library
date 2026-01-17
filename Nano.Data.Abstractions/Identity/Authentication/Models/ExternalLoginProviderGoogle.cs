using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Provider Google.
/// </summary>
public class ExternalLoginProviderGoogle : ExternalLoginProviderImplicit
{
    /// <inheritdoc />
    public ExternalLoginProviderGoogle()
    {
        this.Name = ExternalLogInProviderNames.GOOGLE;
    }
}