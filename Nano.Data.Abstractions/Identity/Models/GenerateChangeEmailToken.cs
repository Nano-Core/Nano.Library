using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to generate a change email token for a user.
/// </summary>
public class GenerateChangeEmailToken
{
    /// <summary>
    /// The new email address to be associated with the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual required string NewEmailAddress { get; set; }
}