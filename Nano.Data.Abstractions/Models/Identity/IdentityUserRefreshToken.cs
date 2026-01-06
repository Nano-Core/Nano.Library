using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity User Refresh Token.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class IdentityUserRefreshToken<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityAuditableNegated
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity User Id.
    /// </summary>
    public virtual TIdentity IdentityUserId { get; set; }

    /// <summary>
    /// Identity User.
    /// </summary>
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; }

    /// <summary>
    /// App Id.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string AppId { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [ProtectedPersonalData]
    [Required]
    [MaxLength(256)]
    public virtual string Value { get; set; }

    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}