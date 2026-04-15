using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to get a specific claim.
/// </summary>
public class GetClaim
{
    /// <summary>
    /// The type of claim to retrieve.
    /// </summary>
    [Required]
    public virtual required string ClaimType { get; set; }
}