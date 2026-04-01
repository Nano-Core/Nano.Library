using Nano.Common.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to change a user's phone number using a confirmation token.
/// </summary>
public class ChangePhoneNumberToken
{
    /// <summary>
    /// The token used to authorize the phone number change.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The new phone number to assign to the user.
    /// </summary>
    [Required]
    [MaxLength(20)]
    [InternationalPhone]
    public virtual string NewPhoneNumber { get; set; } = null!;
}