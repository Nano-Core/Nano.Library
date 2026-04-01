using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to confirm a user's email using a token.
/// </summary>
public class ConfirmEmailToken
{
    /// <summary>
    /// The token used to confirm the user's email address.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}