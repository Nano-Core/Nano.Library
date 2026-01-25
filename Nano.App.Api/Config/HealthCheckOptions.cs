using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring health-check behavior.
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Interval between health-check evaluations, in seconds.
    /// </summary>
    [Required]
    public virtual int EvaluationInterval { get; set; } = 10;

    /// <summary>
    /// Minimum interval between failure notifications, in seconds.
    /// </summary>
    [Required]
    public virtual int FailureNotificationInterval { get; set; } = 60;

    /// <summary>
    /// Maximum number of historical entries per endpoint stored in the UI database.
    /// </summary>
    [Required]
    public virtual int MaximumHistoryEntriesPerEndpoint { get; set; } = 50;

    /// <summary>
    /// Configured web-hooks triggered on health-check events.
    /// </summary>
    [Required]
    public virtual HealthCheckWebHookOptions[] WebHooks { get; set; } = [];
}