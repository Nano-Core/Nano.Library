using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Is Email Address Taken.
/// </summary>
public class IsEmailAddressTaken
{
    /// <summary>
    /// Is Taken.
    /// </summary>
    [Required]
    public virtual bool IsTaken { get; set; }
}