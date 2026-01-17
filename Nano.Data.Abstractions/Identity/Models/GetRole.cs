using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class GetRole : GetRole<Guid>;

/// <summary>
/// Get Role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class GetRole<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Role Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; } = null!;
}