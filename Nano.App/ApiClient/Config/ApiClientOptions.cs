using System;
using Nano.App.Config;
using Nano.Common.Config;

namespace Nano.App.ApiClient.Config;

/// <summary>
/// Represents configuration options for the API client.
/// </summary>
public class ApiClientOptions
{
    /// <summary>
    /// The API host address.
    /// Default is "localhost".
    /// </summary>
    public virtual string Host { get; set; } = "localhost";

    /// <summary>
    /// The root path for the API endpoints.
    /// Default is "api".
    /// </summary>
    public virtual string Root { get; set; } = "api";

    /// <summary>
    /// The port to connect to on the host.
    /// Default is 80.
    /// </summary>
    public virtual int Port { get; set; } = 80;

    /// <summary>
    /// Indicates whether to use SSL (HTTPS) for the connection.
    /// Default is false.
    /// </summary>
    public virtual bool UseSsl { get; set; } = false;

    /// <summary>
    /// The request timeout duration.
    /// Default is 30 seconds.
    /// </summary>
    public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Optional login configuration for authentication.
    /// </summary>
    public virtual LogInRootOptions? LogIn { get; set; }

    /// <summary>
    /// Optional health check configuration for the API client.
    /// </summary>
    public virtual HealthCheckOptions? HealthCheck { get; set; }
}
