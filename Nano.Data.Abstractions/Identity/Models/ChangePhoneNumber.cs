using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to change a user's phone number.
/// </summary>
public class ChangePhoneNumber
{
    /// <summary>
    /// The token used to authorize the phone number change.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;
}