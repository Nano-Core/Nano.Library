using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity;

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