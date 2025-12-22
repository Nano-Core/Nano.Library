using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Health-Check Options
/// </summary>
public class HealthCheckOptions
{
    /// <summary>
    /// Use Health Check UI.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheckUi { get; set; } = true;

    /// <summary>
    /// Evaluation Interval.
    /// The interval between health-checks.
    /// </summary>
    [Required]
    public virtual int EvaluationInterval { get; set; } = 10;

    /// <summary>
    /// Failure Notification Timout.
    /// The minimum number of secoends betweeen failure notificaitons.
    /// </summary>
    [Required]
    public virtual int FailureNotificationInterval { get; set; } = 60;

    /// <summary>
    /// Maximum History Entries Per Endpoint.
    /// The maximum number of historical entries per endpoint in the UI database.
    /// </summary>
    [Required]
    public virtual int MaximumHistoryEntriesPerEndpoint { get; set; } = 50;

    /// <summary>
    /// Web-Hooks.
    /// </summary>
    [Required]
    public virtual HealthCheckWebHookOptions[] WebHooks { get; set; } = [];
}