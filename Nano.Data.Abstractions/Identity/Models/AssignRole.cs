using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AssignRole : AssignRole<Guid>;

/// <summary>
/// Assign Role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class AssignRole<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Role Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string RoleName { get; set; }
}