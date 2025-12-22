using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

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
    /// Use Health Check.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;
}