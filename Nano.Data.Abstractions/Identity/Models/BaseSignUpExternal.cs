using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for external sign-up requests.
/// </summary>
public abstract class BaseSignUpExternal : BaseSignUp;

/// <summary>
/// Base type for external sign-up requests with a strongly typed user.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
public abstract class BaseSignUpExternal<TUser, TIdentity> : BaseSignUpExternal
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user entity created or associated during sign-up.
    /// </summary>
    public virtual TUser User { get; set; } = new();
}

/// <summary>
/// Base type for external sign-up requests with provider-specific data.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
public abstract class BaseSignUpExternal<TProvider, TUser, TIdentity> : BaseSignUpExternal<TUser, TIdentity>
    where TProvider : BaseLogInExternalProvider, new()
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The external authentication provider configuration.
    /// </summary>
    [Required]
    public virtual TProvider Provider { get; set; } = new();
}