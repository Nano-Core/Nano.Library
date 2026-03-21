using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to remove a claim from a user.
/// </summary>
public class RemoveUserClaim
{
    /// <summary>
    /// The claim type to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}