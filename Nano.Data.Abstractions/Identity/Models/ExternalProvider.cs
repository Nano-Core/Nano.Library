using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// 
/// </summary>
public class ExternalProvider
{
    /// <summary>
    /// 
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string LoginProvider { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ProviderKey { get; set; } = null!;
}