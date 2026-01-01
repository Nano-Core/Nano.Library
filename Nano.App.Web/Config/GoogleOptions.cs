using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Google Options.
/// </summary>
public class GoogleOptions
{
    /// <summary>
    /// Client Id.
    /// </summary>
    [Required]
    public virtual string ClientId { get; set; }

    /// <summary>
    /// Client Secret.
    /// </summary>
    [Required]
    public virtual string ClientSecret { get; set; }

    /// <summary>
    /// Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}