using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Base class for external login providers.
/// </summary>
public abstract class BaseLogInExternalProvider
{
    /// <summary>
    /// The unique name of the external provider.
    /// </summary>
    [Required]
    internal string Name { get; set; } = null!;
}