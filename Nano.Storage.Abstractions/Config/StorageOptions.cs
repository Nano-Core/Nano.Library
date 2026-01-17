using Nano.Common.Config;
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
    /// Gets or sets the account or tenant identifier used to authenticate with the storage provider.
    /// </summary>
    /// <remarks>
    ///     The exact meaning of this value depends on the storage provider
    ///     (for example, it may represent an account name, tenant ID, or service identifier).
    /// </remarks>
    [Required]
    public virtual string AccountName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the secret, key, or credential used to authenticate with the storage provider.
    /// </summary>
    /// <remarks>
    ///     This value is treated as sensitive data and should be stored securely.
    /// </remarks>
    [Required]
    public virtual string AccountKey { get; set; } = null!;

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
    public virtual HealthCheckOptions? HealthCheck { get; set; }
}