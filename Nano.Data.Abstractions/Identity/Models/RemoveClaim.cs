using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to remove a claim.
/// </summary>
public class RemoveClaim
{
    /// <summary>
    /// The claim type to remove.
    /// </summary>
    [Required]
    public virtual string ClaimType { get; set; } = null!;
}