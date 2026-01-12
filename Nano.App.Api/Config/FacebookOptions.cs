using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Facebook Options.
/// </summary>
public class FacebookOptions
{
    /// <summary>
    /// App Id.
    /// </summary>
    [Required]
    public virtual string AppId { get; set; }

    /// <summary>
    /// App Secret.
    /// </summary>
    [Required]
    public virtual string AppSecret { get; set; }

    /// <summary>
    /// Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}