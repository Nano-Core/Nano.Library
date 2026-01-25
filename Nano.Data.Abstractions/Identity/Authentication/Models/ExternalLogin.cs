using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an external login reference for a user.
/// </summary>
public class ExternalLogin
{
    /// <summary>
    /// The provider-specific external login key.
    /// </summary>
    [Required]
    public virtual string Key { get; set; } = null!;

    /// <summary>
    /// The external authentication provider information.
    /// </summary>
    [Required]
    public virtual ExternalLoginProvider Provider { get; set; } = new();
}