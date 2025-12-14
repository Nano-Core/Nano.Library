using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class SignUpExternal<TUser> : SignUpExternal<TUser, Guid>
    where TUser : IEntityUser<Guid>;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TUser"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public class SignUpExternal<TUser, TIdentity> : BaseSignUp<TUser, TIdentity>
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Email.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string Email { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ExternalProvider ExternalProvider { get; set; } = new();
}