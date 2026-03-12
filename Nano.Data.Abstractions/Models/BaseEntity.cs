using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntity : BaseEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntity()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities.
/// Implements <see cref="IEntityWritable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntity<TIdentity> : BaseEntityReadOnly<TIdentity>, IEntityWritable
    where TIdentity : IEquatable<TIdentity>;