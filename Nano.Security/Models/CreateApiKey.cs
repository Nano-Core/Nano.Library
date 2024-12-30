using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Create Api Key.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class CreateApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }

    /// <summary>
    /// Expire At.
    /// </summary>
    public virtual DateTimeOffset? ExpireAt { get; set; }
}