using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to remove a role.
/// </summary>
public class RemoveRole
{
    /// <summary>
    /// The name of the role to remove.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; } = null!;
}