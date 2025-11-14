namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Google Options.
/// </summary>
public class GoogleOptions
{
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