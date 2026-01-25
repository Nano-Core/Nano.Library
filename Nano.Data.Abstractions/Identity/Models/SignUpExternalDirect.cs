using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Sign up a user directly without a specific external provider, but by passing authentication data (default identity type is Guid).
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUpExternalDirect<TUser> : BaseSignUpExternal<TUser, Guid>
    where TUser : IEntityUser<Guid>, new();

/// <summary>
/// Sign up a user directly without a specific external provider, but by passing authentication data.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternalDirect<TUser, TIdentity> : BaseSignUpExternal<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Data from the external login provider.
    /// </summary>
    [Required]
    public virtual ExternalLogInData ExternalLogInData { get; set; } = new();
}