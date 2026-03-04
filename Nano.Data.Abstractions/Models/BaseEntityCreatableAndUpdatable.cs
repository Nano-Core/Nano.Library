using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityCreatableAndUpdatable : BaseEntityCreatableAndUpdatable<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityCreatableAndUpdatable"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityCreatableAndUpdatable()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities.
/// Implements <see cref="IEntityCreatable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityCreatableAndUpdatable<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityCreatable
    where TIdentity : IEquatable<TIdentity>
{
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