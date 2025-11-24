using Nano.Data.Abstractions.Config.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Memory Cache.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Max Entries.
    /// The maximum number of cache entries.
    /// </summary>
    [Required]
    public virtual long? MaxEntries { get; set; } = 5000;

    /// <summary>
    /// Expiration Timoeout.
    /// The number of seconds before a cache entry expires.
    /// </summary>
    [Required]
    public virtual int ExpirationTimeoutInSeconds { get; set; } = 300;

    /// <summary>
    /// Expiration Scan Frequency.
    /// The expiration scan interval in seconds.
    /// </summary>
    [Required]
    public virtual int ExpirationScanFrequencyInSeconds { get; set; } = 60;

    /// <summary>
    /// Expiration Mode.
    /// The mode for cache expiration. Absolute or Sliding.
    /// </summary>
    [Required]
    public virtual CacheExpireMode ExpirationMode { get; set; } = CacheExpireMode.Sliding;

    /// <summary>
    /// Ignored Table Names.
    /// The table names that is ignored from the cache.
    /// </summary>
    [Required]
    public virtual string[] IgnoredTableNames { get; set; } = [];
}