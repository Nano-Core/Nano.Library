using Microsoft.AspNetCore.Identity;
using Nano.Common.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Identity User Change Data.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class IdentityUserChangeData<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityAuditableNegated
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity User Id.
    /// </summary>
    [Required]
    public virtual TIdentity IdentityUserId { get; set; } = default!;

    /// <summary>
    /// Identity User.
    /// </summary>
    public virtual IdentityUserEx<TIdentity> IdentityUser { get; set; } = null!;

    /// <summary>
    /// New Email.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    [ProtectedPersonalData]
    public virtual string? NewEmail { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [InternationalPhone]
    [MaxLength(20)]
    [ProtectedPersonalData]
    public virtual string? NewPhoneNumber { get; set; }
}