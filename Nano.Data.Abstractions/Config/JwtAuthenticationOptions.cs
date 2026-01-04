using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Jwt Options (nested class)
/// </summary>
public class JwtAuthenticationOptions
{
    /// <summary>
    /// Refresh Expiration In Hours.
    /// </summary>
    [Required]
    public virtual int RefreshExpirationInHours { get; set; } = 72;
}