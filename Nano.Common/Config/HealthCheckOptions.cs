using System.ComponentModel.DataAnnotations;
using Nano.Common.Mvc.HealthChecks.Enums;

namespace Nano.Common.Config;

/// <summary>
/// Options for configuring health checks in the application.
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Gets or sets the health status level to report when a monitored service is detected as unhealthy.
    /// Defaults to <see cref="HealthStatusLevel.Unhealthy"/>.
    /// </summary>
    [Required]
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
}