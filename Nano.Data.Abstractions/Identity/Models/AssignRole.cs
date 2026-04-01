using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to assign a role.
/// </summary>
public class AssignRole
{
    /// <summary>
    /// The name of the role to assign.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; } = null!;
}