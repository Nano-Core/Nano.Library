using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class AssignUserRole : AssignUserRole<Guid>;

/// <summary>
/// Assign Role.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class AssignUserRole<TIdentity>
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