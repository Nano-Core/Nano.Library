using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Identity Options.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Authentication Options.
    /// </summary>
    [Required]
    public virtual AuthenticationOptions Authentication { get; set; } = new();
}