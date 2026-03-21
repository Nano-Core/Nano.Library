using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to assign a claim to a user.
/// </summary>
public class AssignUserClaim
{
    /// <summary>
    /// The claim type to assign.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The claim value to assign.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; } = null!;
}