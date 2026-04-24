using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to confirm a user's email address.
/// </summary>
public class ConfirmEmail
{
    /// <summary>
    /// The token used to confirm the user's email address.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }
}