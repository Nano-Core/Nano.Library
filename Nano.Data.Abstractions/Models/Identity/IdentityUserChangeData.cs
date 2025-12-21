using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;

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
    public virtual TIdentity IdentityUserId { get; set; }

    /// <summary>
    /// Identity User.
    /// </summary>
    public virtual IdentityUserExt<TIdentity> IdentityUser { get; set; }

    /// <summary>
    /// New Email.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmail { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [InternationalPhone]
    [MaxLength(20)]
    public virtual string NewPhoneNumber { get; set; }
}