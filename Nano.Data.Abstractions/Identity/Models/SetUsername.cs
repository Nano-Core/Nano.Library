using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to update a user's username.
/// </summary>
public class SetUsername
{
    /// <summary>
    /// The new username to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string NewUsername { get; set; }
}