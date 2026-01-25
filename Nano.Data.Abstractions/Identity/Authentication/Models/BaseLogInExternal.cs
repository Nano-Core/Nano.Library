namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Base class for external login requests.
/// </summary>
/// <typeparam name="TProvider">The external provider type.</typeparam>
public abstract class BaseLogInExternal<TProvider> : LogInExternal
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// The external authentication provider.
    /// </summary>
    public virtual TProvider Provider { get; set; } = new();
}