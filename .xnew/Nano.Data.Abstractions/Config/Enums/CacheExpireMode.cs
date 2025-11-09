namespace Nano.Data;

/// <summary>
/// Cache Expire Mode.
/// </summary>
public enum CacheExpireMode
{
    /// <summary>
    /// Defines absolute expiration.
    /// The item will expire after the expiration timeout.
    /// </summary>
    Absolute,

    /// <summary>
    /// Defines sliding expiration.
    /// The expiration timeout will be refreshed on every access.
    /// </summary>
    Sliding,

    /// <summary>
    /// If you do not specify an absolute and/or sliding expiration,
    /// then the item will `theoretically` remain cached indefinitely.
    /// </summary>
    NeverRemove
}