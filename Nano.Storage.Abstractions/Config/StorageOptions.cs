using System.ComponentModel.DataAnnotations;

namespace Nano.Storage.Abstractions.Config;

/// <summary>
/// Represents configuration options for the application's storage.
/// </summary>
/// <remarks>
///     These options provide credentials, logical container naming, and health check behavior for the configured storage provider.
///     The same options object may be used by different storage provider implementations.
/// </remarks>
public class StorageOptions
{
    internal static string SectionName => "Storage";

    /// <summary>
    /// Gets or sets the logical container, share, or bucket name used for file storage.
    /// </summary>
    /// <remarks>
    ///     This value identifies the root namespace within the storage provider where files are stored.
    /// </remarks>
    [Required]
    public virtual string ShareName { get; set; } = null!;

    /// <summary>
    /// Options for configuring health-checks.
    /// </summary>
    public virtual StorageHealthCheckOptions? HealthCheck { get; set; }
}
