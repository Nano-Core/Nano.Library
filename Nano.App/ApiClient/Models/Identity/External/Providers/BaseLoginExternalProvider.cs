using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity.External.Providers;

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