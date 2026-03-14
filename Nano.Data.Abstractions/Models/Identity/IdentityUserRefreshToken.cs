using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents a refresh token for an identity user.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityUserRefreshToken<TIdentity> : BaseEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identity user Id.
    /// </summary>
    [Required]
    public virtual TIdentity IdentityUserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the associated identity user.
    /// </summary>
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;

    /// <summary>
    /// Gets or sets the application Id associated with this token.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string AppId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    [Required]
    [MaxLength(256)]
    [ProtectedPersonalData]
    public virtual string Value { get; set; } = null!;

    /// <summary>
    /// Gets or sets the token expiration timestamp.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}