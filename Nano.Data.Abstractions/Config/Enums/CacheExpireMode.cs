namespace Nano.Data.Abstractions.Config.Enums;

/// <summary>
/// Defines the cache expiration strategy for cached items.
/// </summary>
public enum CacheExpireMode
{
    /// <summary>
    /// Absolute expiration. The cached item will expire after the specified timeout,
    /// regardless of whether it is accessed during that time.
    /// </summary>
    Absolute,

    /// <summary>
    /// Sliding expiration. The expiration timeout will be reset each time the cached item is accessed,
    /// keeping frequently accessed items alive.
    /// </summary>
    Sliding,

    /// <summary>
    /// No automatic expiration. The cached item will remain in the cache indefinitely
    /// unless it is manually removed.
    /// </summary>
    NeverRemove
}