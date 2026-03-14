using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Common.Annotations;

namespace Nano.Data.Abstractions.Models.Identity;

/// <summary>
/// Represents changes made to an identity user (e.g., email or phone number updates).
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityUserChangeData<TIdentity> : BaseEntityIdentity<TIdentity>
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
    /// Gets or sets the new email for the user.
    /// </summary>
    [EmailAddress]
    [MaxLength(256)]
    [ProtectedPersonalData]
    public virtual string? NewEmail { get; set; }

    /// <summary>
    /// Gets or sets the new phone number for the user.
    /// </summary>
    [MaxLength(20)]
    [InternationalPhone]
    [ProtectedPersonalData]
    public virtual string? NewPhoneNumber { get; set; }
}