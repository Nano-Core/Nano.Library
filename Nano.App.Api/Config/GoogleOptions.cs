using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for Google external login.
/// </summary>
public class GoogleOptions
{
    /// <summary>
    /// OAuth Client Id.
    /// </summary>
    [Required]
    public virtual string ClientId { get; set; } = null!;

    /// <summary>
    /// OAuth Client Secret.
    /// </summary>
    [Required]
    public virtual string ClientSecret { get; set; } = null!;

    /// <summary>
    /// OAuth Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}