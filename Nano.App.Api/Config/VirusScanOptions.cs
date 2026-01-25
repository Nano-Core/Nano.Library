using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring virus scanning.
/// </summary>
public class VirusScanOptions
{
    /// <summary>
    /// Hostname of the virus scanning service.
    /// </summary>
    [Required]
    public virtual string Host { get; set; } = "clamav";

    /// <summary>
    /// Port of the virus scanning service.
    /// </summary>
    [Required]
    public virtual int Port { get; set; } = 3310;

    /// <summary>
    /// Health check configuration for the virus scanning service.
    /// </summary>
    public virtual Common.Config.HealthCheckOptions? HealthCheck { get; set; }
}