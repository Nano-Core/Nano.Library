using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to set a new password for a user.
/// </summary>
public class SetPassword
{
    /// <summary>
    /// The new password to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string NewPassword { get; set; } = null!;
}