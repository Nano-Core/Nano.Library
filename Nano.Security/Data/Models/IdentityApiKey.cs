using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models;

namespace Nano.Security.Data.Models;

/// <summary>
/// Identity Api Key.
/// </summary>
public class IdentityApiKey<TIdentity> : BaseEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity User Id.
    /// </summary>
    [Required]
    public virtual TIdentity IdentityUserId { get; set; }

    /// <summary>
    /// Identity User.
    /// </summary>
    public virtual IdentityUser<TIdentity> IdentityUser { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Hash.
    /// </summary>
    [Required]
    public virtual string Hash { get; set; }

    /// <summary>
    /// Created At.
    /// </summary>
    [Required]
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Is Revoked.
    /// </summary>
    public virtual DateTimeOffset? RevokedAt { get; set; }
}