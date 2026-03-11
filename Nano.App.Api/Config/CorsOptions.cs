using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring CORS.
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// Allowed origins.
    /// </summary>
    [Required]
    public virtual string[] AllowedOrigins { get; set; } = [];

    /// <summary>
    /// Allowed HTTP headers.
    /// </summary>
    [Required]
    public virtual string[] AllowedHeaders { get; set; } = [];

    /// <summary>
    /// Allowed HTTP methods.
    /// </summary>
    [Required]
    public virtual string[] AllowedMethods { get; set; } = [];

    /// <summary>
    /// Indicates whether credentials are allowed.
    /// </summary>
    [Required]
    public virtual bool AllowCredentials { get; set; } = false;

    /// <summary>
    /// Origin-specific CORS policies.
    /// </summary>
    [Required]
    public virtual CorsOriginOptions Origin { get; set; } = new();

    /// <summary>
    /// Additional exposed headers.
    /// Nano always expose these headers:
    /// <list>
    ///     <item>RequestId</item>
    ///     <item>TZ</item>
    ///     <item>Content-Disposition</item>
    ///     <item>api-supported-versions</item>
    /// </list>
    /// </summary>
    [Required]
    public virtual IEnumerable<string> ExposedHeaders { get; set; } = [];
}