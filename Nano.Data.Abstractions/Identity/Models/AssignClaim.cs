using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to assign a claim.
/// </summary>
public class AssignClaim
{
    /// <summary>
    /// The claim type to assign.
    /// </summary>
    [Required]
    public virtual required string ClaimType { get; set; }

    /// <summary>
    /// The claim value to assign.
    /// </summary>
    public virtual string? ClaimValue { get; set; }
}