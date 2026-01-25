using System;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for multipart upload limits.
/// </summary>
public class MultipartLimitsOptions
{
    /// <summary>
    /// Maximum allowed upload size in bytes.
    /// </summary>
    public virtual long? MaxUploadBytes { get; init; }

    /// <summary>
    /// Timeout for slow uploads.
    /// </summary>
    public virtual TimeSpan? KeepAliveTimeout { get; init; }
}