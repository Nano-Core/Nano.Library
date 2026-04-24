using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for multipart upload limits.
/// </summary>
public class MultipartLimitsOptions
{
    /// <summary>
    /// Maximum allowed upload size in bytes.
    /// Defaults to 32 MB
    /// </summary>
    [Required]
    public virtual long MaxUploadBytes { get; set; } = 32 * 1024 * 1024;

    /// <summary>
    /// Timeout for slow uploads.
    /// Defaults to 130 seconds.
    /// </summary>
    [Required]
    public virtual TimeSpan KeepAliveTimeout { get; set; } = TimeSpan.FromSeconds(130);
}