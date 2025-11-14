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
    public string Name { get; set; } // BUG: DATA: Was internal, find better way than making this public
}