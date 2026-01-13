namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Log In Data.
/// </summary>
public class ExternalLogInData
{
    /// <summary>
    /// Id.
    /// </summary>
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Email.
    /// </summary>
    public virtual string Email { get; set; } = null!;

    /// <summary>
    /// External Token.
    /// </summary>
    public virtual ExternalLoginTokenData ExternalToken { get; set; } = new();
}