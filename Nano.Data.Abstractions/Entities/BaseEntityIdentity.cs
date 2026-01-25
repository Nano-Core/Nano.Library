using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Entities;

/// <summary>
/// Base class for entities with an identity property.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityIdentity<TIdentity> : IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Required]
    public TIdentity Id { get; set; } = default!;
}