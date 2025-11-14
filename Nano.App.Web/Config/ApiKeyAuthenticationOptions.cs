namespace Nano.Security;

/// <summary>
/// Api Authentication Key Options.
/// </summary>
public class ApiKeyAuthenticationOptions
{
    /// <summary>
    /// Secret.
    /// </summary>
    public virtual string Secret { get; set; } = null;
}