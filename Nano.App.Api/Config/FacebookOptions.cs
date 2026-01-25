using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for Facebook external login.
/// </summary>
public class FacebookOptions
{
    /// <summary>
    /// Facebook App Id.
    /// </summary>
    [Required]
    public virtual string AppId { get; set; } = null!;

    /// <summary>
    /// Facebook App Secret.
    /// </summary>
    [Required]
    public virtual string AppSecret { get; set; } = null!;

    /// <summary>
    /// OAuth Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}