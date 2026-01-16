using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Log In Data.
/// </summary>
public class ExternalLogInData
{
    /// <summary>
    /// Id.
    /// </summary>
    [Required]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Email.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Email { get; set; } = null!;

    /// <summary>
    /// External Token.
    /// </summary>
    public virtual ExternalLoginTokenData ExternalToken { get; set; } = new();
}