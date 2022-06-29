namespace Nano.Security.Models;

/// <summary>
/// External Login Token Data.
/// </summary>
public class ExternalLoginTokenData
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    public virtual string Token { get; set; }

    /// <summary>
    /// Refresh Token.
    /// </summary>
    public virtual string RefreshToken { get; set; }
}