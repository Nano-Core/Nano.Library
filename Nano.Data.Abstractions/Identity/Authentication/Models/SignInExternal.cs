using Nano.Data.Abstractions.Identity.Models;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// 
/// </summary>
public class SignInExternal : BaseSignIn
{
    /// <summary>
    /// 
    /// </summary>
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}