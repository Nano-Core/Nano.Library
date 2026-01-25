namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for general authentication configuration.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Gets or sets the API key authentication options.
    /// </summary>
    public virtual ApiKeyAuthenticationOptions? ApiKey { get; set; }
}