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
    /// OAuth scopes. Must include "openid" (and should include "profile" and "email") so the
    /// token response includes an id_token with the claims the login flow reads (oid/name/email).
    /// </summary>
    [Required]
    public virtual string[] Scopes { get; set; } = [];
}