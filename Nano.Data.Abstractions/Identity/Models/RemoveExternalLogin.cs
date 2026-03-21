using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to remove an external login from a user account.
/// </summary>
public class RemoveExternalLogin
{
    /// <summary>
    /// The name of the external login provider (e.g., Google, Microsoft, Facebook).
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string LoginProvider { get; set; } = null!;
}