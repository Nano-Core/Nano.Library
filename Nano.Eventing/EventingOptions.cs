using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nano.Eventing;

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
    public virtual string Host { get; set; }

    /// <summary>
    /// VHost.
    /// </summary>
    public virtual string VHost { get; set; } = "/";

    /// <summary>
    /// Username.
    /// </summary>
    public virtual string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password.
    /// </summary>
    public virtual string Password { get; set; } = string.Empty;

    /// <summary>
    /// Port.
    /// </summary>
    public virtual ushort Port { get; set; } = 5672;

    /// <summary>
    /// Timeout, in seconds.
    /// </summary>
    public virtual ushort Timeout { get; set; } = 30;

    /// <summary>
    /// Use Ssl.
    /// </summary>
    public virtual bool UseSsl { get; set; } = false;

    /// <summary>
    /// Heartbeat, in seconds.
    /// Default: 60
    /// Zero means no hearbeat requests.
    /// </summary>
    public virtual ushort Heartbeat { get; set; } = 60;

    /// <summary>
    /// Prefetch Count.
    /// Default: 50
    /// </summary>
    public virtual ushort PrefetchCount { get; set; } = 50;

    /// <summary>
    /// Use Health Check.
    /// </summary>
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Unhealthy Status.
    /// </summary>
    public virtual HealthStatus UnhealthyStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// ConnectionString.
    /// </summary>
    public virtual string ConnectionString =>
        string.IsNullOrEmpty(this.Username) || string.IsNullOrEmpty(this.Password)
            ? $"amqp://{this.Host}:{this.Port}{this.VHost}"
            : $"amqp://{this.Username}:{this.Password}@{this.Host}:{this.Port}{this.VHost}";
}