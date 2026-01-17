using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Virus Scan Options.
/// </summary>
public class VirusScanOptions
{
    /// <summary>
    /// Host.
    /// </summary>
    [Required]
    public virtual string Host { get; set; } = "clamav";

    /// <summary>
    /// Port.
    /// </summary>
    [Required]
    public virtual int Port { get; set; } = 3310;

    /// <summary>
    /// Options for configuring health-checks.
    /// </summary>
    public virtual Common.Config.HealthCheckOptions? HealthCheck { get; set; }
}