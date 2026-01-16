using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

// BUG: 000: Add links to documentation and examples readme from triple slash. E.g AddNanoData<>() should link to Nano.Data Readme and related examples / Usages

// BUG: 000: Go through Required / MaxLength / Etc for Entity models and request models - all the way through all layers

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