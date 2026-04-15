using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an external authentication provider.
/// </summary>
public class ExternalLoginProvider
{
    /// <summary>
    /// The unique name of the external provider.
    /// </summary>
    [Required]
    public virtual required string Name { get; set; }

    /// <summary>
    /// The human-readable display name of the provider.
    /// </summary>
    public virtual string? DisplayName { get; set; }
}