using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring response caching.
/// </summary>
public class ResponseCacheOptions
{
    /// <summary>
    /// Maximum cache size in KB. Default is 1 MB.
    /// </summary>
    [Required]
    public virtual int MaxSize { get; set; } = 1024;

    /// <summary>
    /// Maximum cached body size in KB. Default is 100 MB.
    /// </summary>
    [Required]
    public virtual int MaxBodySize { get; set; } = 100 * 1024;

    /// <summary>
    /// Maximum cache duration. Default is 20 minutes.
    /// </summary>
    [Required]
    public virtual TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(20);
}