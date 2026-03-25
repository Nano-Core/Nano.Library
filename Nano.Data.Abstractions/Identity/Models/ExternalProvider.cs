using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents an external authentication provider associated with a user.
/// </summary>
public class ExternalProvider
{
    /// <summary>
    /// The name of the external login provider (e.g., Google, Microsoft, Facebook).
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// The unique key provided by the external provider for the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string UserId { get; set; } = null!;
}