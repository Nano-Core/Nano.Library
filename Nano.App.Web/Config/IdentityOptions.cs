using Nano.Security;

namespace Nano.Web;

/// <summary>
/// Identity Options.
/// </summary>
public class IdentityOptions
{
    /// <summary>
    /// Authentication Options.
    /// </summary>
    public virtual AuthenticationOptions Authentication { get; set; } = new();
}