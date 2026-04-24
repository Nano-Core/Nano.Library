using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents an external login reference for a user.
/// </summary>
public class ExternalLogin
{
    /// <summary>
    /// The provider-specific external login key.
    /// </summary>
    [Required]
    public virtual required string Key { get; set; }

    /// <summary>
    /// The external authentication provider information.
    /// </summary>
    [Required]
    public virtual required ExternalLoginProvider Provider { get; set; }
}