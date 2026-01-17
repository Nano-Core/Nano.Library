using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// 
/// </summary>
public class ContentTypeOptions
{
    /// <summary>
    /// Added X-Content-Type Options header.
    /// </summary>
    [Required]
    public virtual bool NoSniff { get; set; } = true;
}