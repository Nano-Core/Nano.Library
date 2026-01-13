using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class EditApiKey : EditApiKey<Guid>;

/// <summary>
/// Remove Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class EditApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Required]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;
}