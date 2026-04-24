using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring response compression.
/// </summary>
public class ResponseCompressionOptions
{
    /// <summary>
    /// Enable or disable Gzip compression.
    /// </summary>
    [Required]
    public virtual bool UseGzip { get; set; } = true;

    /// <summary>
    /// Enable or disable Brotli compression.
    /// </summary>
    [Required]
    public virtual bool UseBrotli { get; set; } = true;
}