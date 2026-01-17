using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Provider Microsoft.
/// </summary>
public class ExternalLoginProviderMicrosoft : ExternalLoginProviderAuthCode
{
    /// <inheritdoc />
    public ExternalLoginProviderMicrosoft()
    {
        this.Name = ExternalLogInProviderNames.MICROSOFT;
    }
}