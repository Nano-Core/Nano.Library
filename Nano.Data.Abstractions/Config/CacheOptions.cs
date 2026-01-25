using System;
using Nano.Data.Abstractions.Config.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for configuring the in-memory cache behavior.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Gets or sets the maximum number of cache entries allowed.
    /// Defaults to <c>5000</c>.
    /// </summary>
    [Required]
    public virtual long? MaxEntries { get; set; } = 5000;

    /// <summary>
    /// Gets or sets the expiration timeout for each cache entry.
    /// This is the duration after which a cached item will expire.
    /// Defaults to <c>5 minutes</c>.
    /// </summary>
    [Required]
    public virtual TimeSpan ExpirationTimeout { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the interval at which the cache will scan for expired items.
    /// Defaults to <c>1 minute</c>.
    /// </summary>
    [Required]
    public virtual TimeSpan ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the expiration mode for cached items.
    /// Can be <see cref="CacheExpireMode.Absolute"/> or <see cref="CacheExpireMode.Sliding"/>.
    /// Defaults to <see cref="CacheExpireMode.Sliding"/>.
    /// </summary>
    [Required]
    public virtual CacheExpireMode ExpirationMode { get; set; } = CacheExpireMode.Sliding;

    /// <summary>
    /// Gets or sets the names of database tables that should be ignored by the cache.
    /// Defaults to an empty array.
    /// </summary>
    [Required]
    public virtual string[] IgnoredTableNames { get; set; } = Array.Empty<string>();
}
