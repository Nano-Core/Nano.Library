using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents a refresh request for an external authentication provider.
/// </summary>
public class LogInExternalRefresh
{
    /// <summary>
    /// The name of the external authentication provider.
    /// </summary>
    [Required]
    public string ProviderName { get; set; } = null!;

    /// <summary>
    /// The refresh token issued by the external provider.
    /// </summary>
    [Required]
    public virtual string RefreshToken { get; set; } = null!;
}