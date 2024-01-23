using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Generate Change Email Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GenerateChangeEmailToken<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// New Email Address.
    /// </summary>
    [Required]
    [EmailAddress]
    public virtual string NewEmailAddress { get; set; }
}