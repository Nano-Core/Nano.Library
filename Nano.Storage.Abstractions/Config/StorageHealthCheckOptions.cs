using Nano.Common.Config;
using System.ComponentModel.DataAnnotations;

namespace Nano.Storage.Abstractions.Config;

/// <summary>
/// Represents configuration options for storage-related health checks.
/// </summary>
/// <remarks>
///     Extends <see cref="HealthCheckOptions"/> with settings required to validate connectivity and availability of storage resources such as Azure File Shares.
/// </remarks>
public class StorageHealthCheckOptions : HealthCheckOptions
{
    /// <summary>
    /// Gets or sets the Azure storage account name used by the health check.
    /// </summary>
    public virtual string? AccountName { get; set; }
}