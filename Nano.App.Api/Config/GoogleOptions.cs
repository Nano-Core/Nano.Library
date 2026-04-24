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
    public virtual required string ClientId { get; set; }

    /// <summary>
    /// OAuth Client Secret.
    /// </summary>
    [Required]
    public virtual required string ClientSecret { get; set; }

    /// <summary>
    /// OAuth Scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}