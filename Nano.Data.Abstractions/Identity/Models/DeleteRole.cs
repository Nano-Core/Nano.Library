using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to delete a role.
/// </summary>
public class DeleteRole
{
    /// <summary>
    /// The name of the role to delete.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Name { get; set; }
}