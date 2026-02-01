using Nano.App.Config;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// API application options.
/// </summary>
public class ApiOptions : BaseAppOptions
{
    /// <summary>
    /// Number of seconds to wait after a SIGTERM signal before shutting down.
    /// </summary>
    public virtual int ShutdownTimeout { get; set; } = 10;

    /// <summary>
    /// Hosting options.
    /// </summary>
    [Required]
    public virtual HostingOptions Hosting { get; set; } = new();

    /// <summary>
    /// HTTP policy header options.
    /// </summary>
    [Required]
    public virtual HttpPolicyHeaderOptions HttpPolicyHeaders { get; set; } = new();

    /// <summary>
    /// Response caching options.
    /// </summary>
    public virtual ResponseCacheOptions? ResponseCache { get; set; }

    /// <summary>
    /// Response compression options.
    /// </summary>
    public virtual ResponseCompressionOptions? ResponseCompression { get; set; }

    /// <summary>
    /// Session management options.
    /// </summary>
    public virtual SessionOptions? Session { get; set; }

    /// <summary>
    /// Timezone configuration options.
    /// </summary>
    public virtual TimeZoneOptions? TimeZone { get; set; }

    /// <summary>
    /// Virus scanning options.
    /// </summary>
    public virtual VirusScanOptions? VirusScan { get; set; }

    /// <summary>
    /// Health-check configuration options.
    /// </summary>
    public virtual HealthCheckOptions? HealthCheck { get; set; }

    /// <summary>
    /// API documentation options.
    /// </summary>
    public virtual DocumentationOptions? Documentation { get; set; }

    /// <summary>
    /// Identity configuration options.
    /// </summary>
    public virtual IdentityOptions? Identity { get; set; }
}