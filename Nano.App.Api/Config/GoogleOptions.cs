using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Google Options.
/// </summary>
public class GoogleOptions
{
    /// <summary>
    /// Client Id.
    /// </summary>
    [Required]
    public virtual string ClientId { get; set; } = null!;

    /// <summary>
    /// Client Secret.
    /// </summary>
    [Required]
    public virtual string ClientSecret { get; set; } = null!;

    /// <summary>
    /// Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}