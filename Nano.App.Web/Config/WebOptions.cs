using System.ComponentModel.DataAnnotations;
using Nano.App.Config;

namespace Nano.App.Web.Config;

/// <summary>
/// Web Options.
/// </summary>
public class WebOptions : BaseAppOptions
{
    /// <summary>
    /// Default Time Zone.
    /// </summary>
    [Required]
    public virtual string DefaultTimeZone { get; set; } = "UTC";

    /// <summary>
    /// Shutdown Timeout.
    /// The number of seconds the application waits after receiving a SIGTERM signal, before shutting down.
    /// </summary>
    public virtual int ShutdownTimeout { get; set; } = 10;

    /// <summary>
    /// Hosting.
    /// </summary>
    [Required]
    public virtual HostingOptions Hosting { get; set; } = new();

    /// <summary>
    /// Hosting.
    /// </summary>
    [Required]
    public virtual HttpPolicyHeaderOptions HttpPolicyHeaders { get; set; } = new();

    /// <summary>
    /// Authentication Options.
    /// </summary>
    public virtual IdentityOptions Identity { get; set; }

    /// <summary>
    /// Response Cache.
    /// Options for caching responses.
    /// </summary>
    public virtual ResponseCacheOptions ResponseCache { get; set; }

    /// <summary>
    /// Session.
    /// Settings for session behavior.
    /// </summary>
    public virtual SessionOptions Session { get; set; }

    /// <summary>
    /// Virus Scan.
    /// </summary>
    public virtual VirusScanOptions VirusScan { get; set; }

    /// <summary>
    /// Health-Check.
    /// </summary>
    public virtual HealthCheckOptions HealthCheck { get; set; }

    /// <summary>
    /// Documentation.
    /// </summary>
    public virtual DocumentationOptions Documentation { get; set; }
}