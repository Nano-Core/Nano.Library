using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Response Compression Options.
/// </summary>
public class ResponseCompressionOptions
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual bool UseGzip { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual bool UseBrotli { get; set; } = true;
}