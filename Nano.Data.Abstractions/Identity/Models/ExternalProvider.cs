using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

// BUG: Add links to documentation and examples readme from triple slash. E.g AddNanoData<>() should link to Nano.Data Readme and related examples / Usages

// BUG: Go through Required / MaxLength / Etc for Entity models and request models - all the way through all layers

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