using System;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Mvc.HealthChecks.Enums;

namespace Nano.Eventing.Abstractions.Config;

/// <summary>
/// Eventing Options.
/// </summary>
public class EventingOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public static string SectionName => "Eventing";

    /// <summary>
    /// Host.
    /// </summary>
    [Required]
    public virtual string Host { get; set; }

    /// <summary>
    /// VHost.
    /// </summary>
    [Required]
    public virtual string VHost { get; set; } = "/";

    /// <summary>
    /// Username.
    /// </summary>
    public virtual string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public virtual string Password { get; set; }

    /// <summary>
    /// Port.
    /// </summary>
    [Required]
    public virtual ushort Port { get; set; } = 5672;

    /// <summary>
    /// Timeout, in seconds.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMicroseconds(30);

    /// <summary>
    /// Use Ssl.
    /// </summary>
    [Required]
    public virtual bool UseSsl { get; set; } = false;

    /// <summary>
    /// Heartbeat, in seconds.
    /// Default: 60
    /// Zero means no hearbeat requests.
    /// </summary>
    [Required]
    public virtual ushort Heartbeat { get; set; } = 60;

    /// <summary>
    /// Prefetch Count.
    /// Default: 50
    /// </summary>
    [Required]
    public virtual ushort PrefetchCount { get; set; } = 50;

    /// <summary>
    /// Use Health Check.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Unhealthy Status.
    /// </summary>
    [Required]
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
}