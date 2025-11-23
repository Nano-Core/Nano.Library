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
public class SignUpExternal<TUser, TIdentity> : BaseSignUp
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User.
    /// </summary>
    [Required]
    public virtual TUser User { get; set; }

    /// <summary>
    /// External Log-In Data.
    /// </summary>
    [Required]
    public virtual ExternalLogInData ExternalLogInData { get; set; }
}