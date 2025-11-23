using Nano.Eventing;

namespace Nano.Storage.Abstractions.Config;

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
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;

    /// <summary>
    /// Connectionstring.
    /// </summary>
    public virtual string Connectionstring => this.AccountName == null || this.AccountKey == null 
        ? null 
        : $"DefaultEndpointsProtocol=https;AccountName={this.AccountName};AccountKey={this.AccountKey};EndpointSuffix=core.windows.net";
}