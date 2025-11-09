using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

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