using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to replace a claim with a new value.
/// </summary>
public class ReplaceClaim
{
    /// <summary>
    /// The claim type to replace.
    /// </summary>
    [Required]
    public virtual required string ClaimType { get; set; }

    /// <summary>
    /// The new value for the claim.
    /// </summary>
    public virtual string? NewClaimValue { get; set; }
}