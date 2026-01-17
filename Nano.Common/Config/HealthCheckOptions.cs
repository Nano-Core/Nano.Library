using System.ComponentModel.DataAnnotations;
using Nano.Common.Mvc.HealthChecks.Enums;

namespace Nano.Common.Config;

/// <summary>
///
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Health status level to report when the eventing service is detected as unhealthy.
    /// Default is <see cref="HealthStatusLevel.Unhealthy"/>.
    /// </summary>
    [Required]
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
}