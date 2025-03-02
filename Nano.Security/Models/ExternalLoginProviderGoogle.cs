namespace Nano.Security.Models;

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