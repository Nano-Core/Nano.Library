using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Cors Options.
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Allowed Origins.
    /// </summary>
    [Required]
    public virtual string[] AllowedOrigins { get; set; } = [];

    /// <summary>
    /// Allowed Headers.
    /// </summary>
    [Required]
    public virtual string[] AllowedHeaders { get; set; } = [];

    /// <summary>
    /// Allowed methods.
    /// </summary>
    [Required]
    public virtual string[] AllowedMethods { get; set; } = [];

    /// <summary>
    /// Allow Credentials.
    /// </summary>
    [Required]
    public virtual bool AllowCredentials { get; set; } = true;

    /// <summary>
    /// Origin.
    /// </summary>
    [Required]
    public virtual CorsOriginOptions Origin { get; set; } = new();
}