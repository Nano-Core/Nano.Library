using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Create Role.
/// </summary>
public class CreateRole
{
    /// <summary>
    /// Role Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; }
}