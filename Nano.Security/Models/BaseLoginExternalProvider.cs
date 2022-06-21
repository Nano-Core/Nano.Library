using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Base LogIn External Provider (abstract).
/// </summary>
public abstract class BaseLogInExternalProvider
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    internal string Name { get; set; }
}