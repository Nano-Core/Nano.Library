using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class RevokeApiKey : RevokeApiKey<Guid>;

/// <summary>
/// Remove Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class RevokeApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Required]
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Revoke At.
    /// </summary>
    [Required]
    public virtual DateTimeOffset? RevokeAt { get; set; }
}