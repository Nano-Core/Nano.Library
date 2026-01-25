using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for identity configuration.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Authentication-related options.
    /// </summary>
    [Required]
    public virtual AuthenticationOptions Authentication { get; set; } = new();
}