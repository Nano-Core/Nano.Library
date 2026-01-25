using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for Microsoft authentication.
/// </summary>
public class MicrosoftOptions
{
    /// <summary>
    /// Tenant Id.
    /// </summary>
    [Required]
    public virtual string TenantId { get; set; } = null!;

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
    /// OAuth scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}