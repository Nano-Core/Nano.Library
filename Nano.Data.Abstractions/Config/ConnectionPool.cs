using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for configuring the cache behavior.
/// </summary>
public class ConnectionPool
{
    /// <summary>
    /// Get or set the pool size of the connection pool.
    /// </summary>
    [Required]
    public virtual int PoolSize { get; set; } = 1024;
}