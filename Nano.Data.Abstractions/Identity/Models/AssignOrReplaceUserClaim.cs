using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to add or replace a claim on a user.
/// </summary>
public class AssignOrReplaceUserClaim
{
    /// <summary>
    /// The claim type to add or replace.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The claim value to add or replace.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; } = null!;
}