using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base Sign-Up (abstract).
/// </summary>
public abstract class BaseSignUp
{
    /// <summary>
    /// Roles.
    /// Additional Roles to add to the user, besides the default roles in the configuration.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// Claims.
    /// Additonal claims to add to the user.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// 
/// </summary>
public abstract class BaseSignUp<TUser, TIdentity> : BaseSignUp
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User.
    /// </summary>
    [Required]
    public virtual TUser User { get; set; } = default!;
}