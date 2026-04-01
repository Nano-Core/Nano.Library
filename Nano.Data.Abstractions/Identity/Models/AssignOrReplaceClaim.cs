using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to add or replace a claim.
/// </summary>
public class AssignOrReplaceClaim
{
    /// <summary>
    /// The claim type to add or replace.
    /// </summary>
    [Required]
    public virtual string ClaimType { get; set; } = null!;

    /// <summary>
    /// The claim value to add or replace.
    /// </summary>
    [Required]
    public virtual string ClaimValue { get; set; } = null!;
}