using System;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Annotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class SignUp<TUser> : SignUp<TUser, Guid>
    where TUser : IEntityUser<Guid>;

/// <summary>
/// Represents a request to sign up a new user.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class SignUp<TUser, TIdentity> : BaseSignUp<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string EmailAddress { get; set; } = null!;

    /// <summary>
    /// The user's phone number (optional, international format supported).
    /// </summary>
    [InternationalPhone]
    public virtual string? PhoneNumber { get; set; }

    /// <summary>
    /// The user's chosen username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Username { get; set; } = null!;

    /// <summary>
    /// The user's password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; } = null!;

    /// <summary>
    /// Confirmation of the user's password. Must match <see cref="Password"/>.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [Compare(nameof(Password))]
    public virtual string ConfirmPassword { get; set; } = null!;
}