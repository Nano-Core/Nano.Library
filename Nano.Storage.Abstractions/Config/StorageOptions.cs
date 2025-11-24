using System.ComponentModel.DataAnnotations;
using Nano.Common.Enums;

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
    [Required]
    public virtual string AccountName { get; set; }

    /// <summary>
    /// Account Key.
    /// </summary>
    [Required]
    public virtual string AccountKey { get; set; }

    /// <summary>
    /// Share Name.
    /// </summary>
    [Required]
    public virtual string ShareName { get; set; }

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

    /// <summary>
    /// Connectionstring.
    /// </summary>
    public virtual string Connectionstring => this.AccountName == null || this.AccountKey == null 
        ? null 
        : $"DefaultEndpointsProtocol=https;AccountName={this.AccountName};AccountKey={this.AccountKey};EndpointSuffix=core.windows.net";
}