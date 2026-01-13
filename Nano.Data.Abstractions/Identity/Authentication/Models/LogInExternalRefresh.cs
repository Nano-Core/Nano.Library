using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Log In External Refresh.
/// </summary>
public class LogInExternalRefresh
{
    /// <summary>
    /// Provider Name.
    /// </summary>
    [Required]
    public string ProviderName { get; set; } = null!;

    /// <summary>
    /// Refresh Token.
    /// </summary>
    [Required]
    public virtual string RefreshToken { get; set; } = null!;
}