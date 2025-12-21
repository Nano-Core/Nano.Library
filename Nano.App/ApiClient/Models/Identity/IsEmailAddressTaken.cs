using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity;

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