namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Authentication Options.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Api Key.
    /// </summary>
    public virtual ApiKeyAuthenticationOptions ApiKey { get; set; }
}