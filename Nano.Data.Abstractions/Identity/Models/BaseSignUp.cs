using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for sign-up requests.
/// </summary>
public abstract class BaseSignUp
{
    /// <summary>
    /// Additional roles to assign to the user, beyond the default configured roles.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// Additional claims to assign to the user.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Base type for sign-up requests with a strongly typed user.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The user identity type.</typeparam>
public abstract class BaseSignUp<TUser, TIdentity> : BaseSignUp
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user entity to be created during sign-up.
    /// </summary>
    [Required]
    public virtual TUser User { get; set; } = default!;
}