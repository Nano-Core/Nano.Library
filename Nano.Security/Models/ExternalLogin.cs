namespace Nano.Security.Models;

/// <summary>
/// External Login.
/// </summary>
public class ExternalLogin
{
    /// <summary>
    /// Key.
    /// </summary>
    public virtual string Key { get; set; }

    /// <summary>
    /// Provider.
    /// </summary>
    public virtual ExternalLoginProvider Provider { get; set; } = new();
}