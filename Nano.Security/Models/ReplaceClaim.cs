using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Replace Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ReplaceClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Claim Type.
    /// </summary>
    [Required]
    public virtual string ClaimType { get; set; }

    /// <summary>
    /// New Claim Value.
    /// </summary>
    [Required]
    public virtual string NewClaimValue { get; set; }
}