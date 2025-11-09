using Nano.Security.Const;
using System;

namespace Nano.Security;

/// <summary>
/// Authentication Options.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// Jwt Options.
    /// </summary>
    public virtual JwtAuthenticationOptions Jwt { get; set; } = new();

    /// <summary>
    /// Api Key.
    /// </summary>
    public virtual ApiKeyOptions ApiKey { get; set; } = new();
}