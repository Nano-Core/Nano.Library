using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nano.Storage;

/// <summary>
/// Storage Options
/// </summary>
public class StorageOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public static string SectionName => "Storage";

    /// <summary>
    /// Account Name.
    /// </summary>
    public virtual string AccountName { get; set; }

    /// <summary>
    /// Account Key.
    /// </summary>
    public virtual string AccountKey { get; set; }

    /// <summary>
    /// Share Name.
    /// </summary>
    public virtual string ShareName { get; set; }

    /// <summary>
    /// Use Health Check.
    /// </summary>
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Unhealthy Status.
    /// </summary>
    public virtual HealthStatus UnhealthyStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// Connection String.
    /// </summary>
    public virtual string ConnectionString => this.AccountName == null || this.AccountKey == null 
        ? null 
        : $"DefaultEndpointsProtocol=https;AccountName={this.AccountName};AccountKey={this.AccountKey};EndpointSuffix=core.windows.net";
}