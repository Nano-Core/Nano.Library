using System;
using System.ComponentModel.DataAnnotations;
using Nano.Models.Interfaces;

namespace Nano.Security.Models;

/// <summary>
/// Sign Up.
/// </summary>
public abstract class SignUp : BaseSignUp
{
    /// <summary>
    /// Email.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string EmailAddress { get; set; }

    /// <summary>
    /// Username.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; }

    /// <summary>
    /// Confirm Password.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [Compare("Password")]
    public virtual string ConfirmPassword { get; set; }
}

/// <summary>
/// Sign Up.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
public class SignUp<TUser> : SignUp<TUser, Guid>
    where TUser : IEntityUser<Guid>
{

}

/// <summary>
/// Sign Up.
/// </summary>
/// <typeparam name="TUser">The user type.</typeparam>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public class SignUp<TUser, TIdentity> : SignUp
    where TUser : IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User.
    /// </summary>
    [Required]
    public virtual TUser User { get; set; }
}