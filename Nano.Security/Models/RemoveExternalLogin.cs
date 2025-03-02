using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Remove External Login.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class RemoveExternalLogin<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// External Login.
    /// </summary>
    public virtual ExternalLogin ExternalLogin { get; set; } = new();
}