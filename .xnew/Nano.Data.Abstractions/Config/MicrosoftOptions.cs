namespace Nano.Security;

/// <summary>
/// Microsoft Options.
/// </summary>
public class MicrosoftOptions
{
    /// <summary>
    /// Tenant Id.
    /// </summary>
    public virtual string TenantId { get; set; }

    /// <summary>
    /// Client Id.
    /// </summary>
    public virtual string ClientId { get; set; }

    /// <summary>
    /// Client Secret.
    /// </summary>
    public virtual string ClientSecret { get; set; }

    /// <summary>
    /// Scopes.
    /// </summary>
    public virtual string[] Scopes { get; set; } = [];
}