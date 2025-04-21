using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Interfaces;

namespace Nano.Models.Data;

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
    public virtual IdentityUser<TIdentity> IdentityUser { get; set; }

    /// <summary>
    /// New Email.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    public virtual string NewEmail { get; set; }

    /// <summary>
    /// New Phone Number.
    /// </summary>
    [Phone]
    [MaxLength(20)]
    public virtual string NewPhoneNumber { get; set; }
}