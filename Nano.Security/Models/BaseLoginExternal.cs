namespace Nano.Security.Models;

/// <summary>
/// Base LogIn External (abstract).
/// </summary>
/// <typeparam name="TProvider">The provider type.</typeparam>
public abstract class BaseLogInExternal<TProvider> : LogInExternal
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// Provider.
    /// </summary>
    public TProvider Provider { get; set; } = new();
}