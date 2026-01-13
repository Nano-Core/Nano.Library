namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login Token Data.
/// </summary>
public class ExternalLoginTokenData
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Token.
    /// </summary>
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// Refresh Token.
    /// </summary>
    public virtual string? RefreshToken { get; set; }
}