using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to change a user's email address using a confirmation token.
/// </summary>
public class ChangeEmailToken
{
    /// <summary>
    /// The token used to authorize the email change.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }

    /// <summary>
    /// The new email address to assign to the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual required string NewEmailAddress { get; set; }
}