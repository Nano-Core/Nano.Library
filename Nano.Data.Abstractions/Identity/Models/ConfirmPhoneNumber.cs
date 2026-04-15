using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to confirm a user's phone number.
/// </summary>
public class ConfirmPhoneNumber
{
    /// <summary>
    /// The token used to confirm the user's phone number.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }
}