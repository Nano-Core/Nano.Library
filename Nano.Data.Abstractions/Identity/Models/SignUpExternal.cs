using Nano.Data.Abstractions.Entities.Abstractions;
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
    /// The user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string Email { get; set; } = null!;

    /// <summary>
    /// The external provider information.
    /// </summary>
    [Required]
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}