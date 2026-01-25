using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GetRoleClaim : GetRoleClaim<Guid>;

/// <summary>
/// Represents a request to get a claim for a role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class GetRoleClaim<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the role.
    /// </summary>
    [Required]
    public virtual TIdentity RoleId { get; set; } = default!;

    /// <summary>
    /// The type of claim to retrieve.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ClaimType { get; set; } = null!;
}