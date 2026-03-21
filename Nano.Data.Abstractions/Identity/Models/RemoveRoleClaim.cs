using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to remove a claim from a role.
/// </summary>
public class RemoveRoleClaim
{
    /// <summary>
    /// The claim type to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}