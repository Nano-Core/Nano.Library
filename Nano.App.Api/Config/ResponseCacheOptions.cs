using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Response Cache Options.
/// </summary>
public class ResponseCacheOptions
{
    /// <summary>
    /// Max Size.
    /// Default 1 MB.
    /// </summary>
    [Required]
    public virtual int MaxSize { get; set; } = 1024;

    /// <summary>
    /// Max Body Size.
    /// Default 100 MB.
    /// </summary>
    [Required]
    public virtual int MaxBodySize { get; set; } = 100 * 1024;

    /// <summary>
    /// Max Age.
    /// Default 20 minutes.
    /// </summary>
    [Required]
    public virtual TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(20);
}