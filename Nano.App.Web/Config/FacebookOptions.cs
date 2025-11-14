namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Facebook Options.
/// </summary>
public class FacebookOptions
{
    /// <summary>
    /// App Id.
    /// </summary>
    public virtual string AppId { get; set; }

    /// <summary>
    /// App Secret.
    /// </summary>
    public virtual string AppSecret { get; set; }

    /// <summary>
    /// Scopes.
    /// </summary>
    public virtual string[] Scopes { get; set; } = [];
}