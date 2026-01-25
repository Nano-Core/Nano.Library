using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for X-Content-Type-Options header.
/// </summary>
public class ContentTypeOptions
{
    /// <summary>
    /// If true, prevents MIME type sniffing.
    /// </summary>
    [Required]
    public virtual bool NoSniff { get; set; } = true;
}