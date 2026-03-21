using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to change a user's email address.
/// </summary>
public class ChangeEmail
{
    /// <summary>
    /// The token used to authorize the email change.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}