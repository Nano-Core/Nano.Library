using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Api Authentication Key Options.
/// </summary>
public class ApiKeyAuthenticationOptions
{
    /// <summary>
    /// Secret.
    /// </summary>
    [Required]
    public virtual string Secret { get; set; }
}