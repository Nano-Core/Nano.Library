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
    public virtual required string AppId { get; set; }

    /// <summary>
    /// Facebook App Secret.
    /// </summary>
    [Required]
    public virtual required string AppSecret { get; set; }

    /// <summary>
    /// OAuth Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}