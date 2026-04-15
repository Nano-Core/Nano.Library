using Nano.Common.Annotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SignUpExternal<TUser> : SignUpExternal<TUser, Guid>
    where TUser : IEntityUser<Guid>;

/// <summary>
/// Represents a request to sign up a user using an external provider.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUpExternal<TUser, TIdentity> : BaseSignUp<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user's username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual required string EmailAddress { get; set; }

    /// <summary>
    /// The user's phone number (optional, international format supported).
    /// </summary>
    [InternationalPhone]
    public virtual required string? PhoneNumber { get; set; }

    /// <summary>
    /// The external provider information.
    /// </summary>
    [Required]
    public virtual required ExternalProvider ExternalProvider { get; set; }
}

/// <summary>
/// For external sign-up requests with provider-specific data.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public class SignUpExternal<TFlow, TUser, TIdentity> : BaseSignUp<TUser, TIdentity>
    where TFlow : BaseAuthFlow
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The external authentication flow.
    /// </summary>
    [Required]
    public virtual required TFlow Flow { get; set; }
}