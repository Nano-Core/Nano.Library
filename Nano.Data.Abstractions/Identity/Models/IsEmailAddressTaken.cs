using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents the result of a check to determine whether an email address is already taken.
/// </summary>
public class IsEmailAddressTaken
{
    /// <summary>
    /// Indicates whether the email address is already taken.
    /// </summary>
    [Required]
    public virtual bool IsTaken { get; set; }
}