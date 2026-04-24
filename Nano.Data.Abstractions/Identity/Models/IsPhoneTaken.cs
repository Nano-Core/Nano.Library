using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents the result of a check to determine whether a phone number is already taken.
/// </summary>
public class IsPhoneNumberTaken
{
    /// <summary>
    /// Indicates whether the phone number is already taken.
    /// </summary>
    [Required]
    public virtual bool IsTaken { get; set; }
}