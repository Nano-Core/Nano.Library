using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to replace a role's claim with a new value.
/// </summary>
public class ReplaceRoleClaim
{
    /// <summary>
    /// The claim type to replace.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The new value for the claim.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; } = null!;
}