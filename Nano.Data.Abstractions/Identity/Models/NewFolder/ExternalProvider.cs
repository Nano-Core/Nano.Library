namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// 
/// </summary>
public class ExternalProvider
{
    /// <summary>
    /// 
    /// </summary>
    public virtual string LoginProvider { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual string ProviderKey { get; set; }
}