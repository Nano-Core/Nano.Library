using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Entities;

/// <summary>
/// Base class for entities with a writable state.
/// Implements <see cref="IEntityWritable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntity<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    /// <summary>
    /// Soft delete flag.
    /// Zero means active; any value greater than zero indicates the entity is deleted.
    /// </summary>
    [Required]
    public virtual long IsDeleted { get; set; } = 0L;

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    [Required]
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}