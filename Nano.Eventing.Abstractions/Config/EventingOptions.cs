using Nano.Common.Mvc.HealthChecks.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Eventing.Abstractions.Config;

/// <summary>
/// Configuration options for eventing in Nano applications.
/// <para>
/// These options are used by eventing providers to configure connections,
/// message handling, and optional health checks.
/// They are intentionally agnostic to the underlying messaging system.
/// </para>
/// </summary>
public class EventingOptions
{
    internal static string SectionName => "Eventing";

    /// <summary>
    /// The hostname or IP address of the event broker or messaging server.
    /// </summary>
    [Required]
    public virtual string Host { get; set; } = null!;

    /// <summary>
    /// The virtual host or namespace on the broker to connect to, if applicable.
    /// Default is '/'.
    /// </summary>
    [Required]
    public virtual string VHost { get; set; } = "/";

    /// <summary>
    /// Username for authenticating with the broker.
    /// </summary>
    public virtual string Username { get; set; } = null!;

    /// <summary>
    /// Password for authenticating with the broker.
    /// </summary>
    public virtual string Password { get; set; } = null!;

    /// <summary>
    /// Port to connect to on the broker.
    /// Default is 5672.
    /// </summary>
    [Required]
    public virtual ushort Port { get; set; } = 5672;

    /// <summary>
    /// Connection timeout for the broker, in seconds.
    /// </summary>
    [Required]
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Indicates whether to use SSL/TLS when connecting to the broker.
    /// Default is false.
    /// </summary>
    [Required]
    public virtual bool UseSsl { get; set; } = false;

    /// <summary>
    /// Heartbeat or keep-alive interval in seconds to maintain the connection.
    /// Default is 60. Set to zero to disable heartbeat/keep-alive.
    /// </summary>
    [Required]
    public virtual ushort Heartbeat { get; set; } = 60;

    /// <summary>
    /// Prefetch count for consuming messages.
    /// Controls how many messages can be fetched at once for processing.
    /// Default is 50.
    /// </summary>
    [Required]
    public virtual ushort PrefetchCount { get; set; } = 50;

    // BUG: 000: Merge into HealthCheckOptions (Eventing, Storage, Data, ApiClient, more?)
    ///// <summary>
    /////
    ///// </summary>
    //public virtual HealthCheckOptions? HealthCheck { get; set; }
    ///// <summary>
    /////
    ///// </summary>
    //public class HealthCheckOptions
    //{
    //    /// <summary>
    //    /// Health status level to report when the eventing service is detected as unhealthy.
    //    /// Default is <see cref="HealthStatusLevel.Unhealthy"/>.
    //    /// </summary>
    //    [Required]
    //    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
    //}

    /// <summary>
    /// Indicates whether to register a health check for the eventing service.
    /// Default is true.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Health status level to report when the eventing service is detected as unhealthy.
    /// Default is <see cref="HealthStatusLevel.Unhealthy"/>.
    /// </summary>
    [Required]
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
}