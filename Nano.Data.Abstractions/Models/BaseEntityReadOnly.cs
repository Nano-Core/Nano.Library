using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityReadOnly : BaseEntityReadOnly<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityReadOnly()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class read only for entities.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityReadOnly<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityReadOnly<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    [Required]
    public virtual long IsDeleted { get; set; } = 0L;

    /// <inheritdoc />
    [Required]
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}