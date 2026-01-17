using System;
using Nano.App.Config;
using Nano.Common.Config;

namespace Nano.App.ApiClient.Config;

/// <summary>
/// Api Options.
/// </summary>
public class ApiClientOptions
{
    /// <summary>
    /// Host.
    /// </summary>
    public virtual string Host { get; set; } = "localhost";

    /// <summary>
    /// Root.
    /// </summary>
    public virtual string Root { get; set; } = "api";

    /// <summary>
    /// Port.
    /// </summary>
    public virtual int Port { get; set; } = 80;

    /// <summary>
    /// Use Ssl.
    /// </summary>
    public virtual bool UseSsl { get; set; } = false;

    /// <summary>
    /// Timeout In Seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// LogIn.
    /// </summary>
    public virtual LogInRootOptions? LogIn { get; set; }

    /// <summary>
    /// Health Check.
    /// </summary>
    public virtual HealthCheckOptions? HealthCheck { get; set; }
}