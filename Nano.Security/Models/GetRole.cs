using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

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
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Role Name.
    /// </summary>
    [Required]
    public virtual string RoleName { get; set; }
}