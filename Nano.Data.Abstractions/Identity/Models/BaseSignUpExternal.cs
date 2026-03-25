using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for external sign-up requests with a strongly typed user.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
public abstract class BaseSignUpExternal<TUser, TIdentity> : BaseSignUp<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;

/// <summary>
/// Base type for external sign-up requests with provider-specific data.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public abstract class BaseSignUpExternal<TProvider, TFlow, TUser, TIdentity> : BaseSignUpExternal<TUser, TIdentity>
    where TProvider : BaseExternalProvider
    where TFlow : BaseAuthFlow
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The external authentication provider configuration.
    /// </summary>
    [Required]
    public virtual TProvider Provider { get; set; } = null!;

    /// <summary>
    /// The flow used for authentication.
    /// </summary>
    [Required]
    public virtual TFlow Flow { get; set; } = null!;
}