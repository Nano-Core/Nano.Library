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
    public virtual string LoginProvider { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public virtual string ProviderKey { get; set; } = null!;
}