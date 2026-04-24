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
    public virtual required string TenantId { get; set; }

    /// <summary>
    /// Client Id.
    /// </summary>
    [Required]
    public virtual required string ClientId { get; set; }

    /// <summary>
    /// Client Secret.
    /// </summary>
    [Required]
    public virtual required string ClientSecret { get; set; }

    /// <summary>
    /// OAuth scopes.
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}