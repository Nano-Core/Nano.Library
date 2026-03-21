using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to edit an API key.
/// </summary>
public class EditApiKey
{
    /// <summary>
    /// The updated name for the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;
}