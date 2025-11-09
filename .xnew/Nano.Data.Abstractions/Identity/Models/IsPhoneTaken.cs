using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Is Phone Number Taken.
/// </summary>
public class IsPhoneNumberTaken
{
    /// <summary>
    /// Is Taken.
    /// </summary>
    [Required]
    public virtual bool IsTaken { get; set; }
}