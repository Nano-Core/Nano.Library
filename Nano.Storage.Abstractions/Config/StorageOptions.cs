using System.ComponentModel.DataAnnotations;
using Nano.Common.Mvc.HealthChecks.Enums;

namespace Nano.Storage.Abstractions.Config;

/// <summary>
/// Represents configuration options for the application's storage subsystem.
/// </summary>
/// <remarks>
///     These options provide credentials, logical container naming, and health check behavior for the configured storage provider.
///     The same options object may be used by different storage provider implementations.
/// </remarks>
public class StorageOptions
{
    /// <summary>
    /// The configuration section name used to bind <see cref="StorageOptions"/>.
    /// </summary>
    public static string SectionName => "Storage";

    /// <summary>
    /// Gets or sets the account or tenant identifier used to authenticate with the storage backend.
    /// </summary>
    /// <remarks>
    ///     The exact meaning of this value depends on the storage provider
    ///     (for example, it may represent an account name, tenant ID, or service identifier).
    /// </remarks>
    [Required]
    public virtual string AccountName { get; set; }

    /// <summary>
    /// Gets or sets the secret, key, or credential used to authenticate with the storage backend.
    /// </summary>
    /// <remarks>
    ///     This value is treated as sensitive data and should be stored securely.
    /// </remarks>
    [Required]
    public virtual string AccountKey { get; set; }

    /// <summary>
    /// Gets or sets the logical container, share, or bucket name used for file storage.
    /// </summary>
    /// <remarks>
    ///     This value identifies the root namespace within the storage backend where files are stored.
    /// </remarks>
    [Required]
    public virtual string ShareName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether storage health checks should be registered.
    /// </summary>
    /// <remarks>
    ///     When enabled, the configured storage provider may register a health check that validates connectivity and availability of the storage backend.
    /// </remarks>
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Gets or sets the health status reported when the storage backend is unavailable.
    /// </summary>
    /// <remarks>
    /// This value controls the severity level reported by storage health checks when failures are detected.
    /// </remarks>
    public virtual HealthStatusLevel UnhealthyStatus { get; set; } = HealthStatusLevel.Unhealthy;
}