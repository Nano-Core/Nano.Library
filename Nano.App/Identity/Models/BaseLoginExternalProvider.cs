using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base LogIn External Provider (abstract).
/// </summary>
public abstract class BaseLogInExternalProvider
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public string Name { get; set; } // BUG: 000: WILL BE FIXED WHEN WE MOVE MODELS: DATA: Was internal, find better way than making this public
}