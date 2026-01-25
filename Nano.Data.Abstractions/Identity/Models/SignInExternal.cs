namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to sign in using an external provider.
/// </summary>
public class SignInExternal : BaseSignIn
{
    /// <summary>
    /// The external provider information.
    /// </summary>
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}