using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityDeletable : BaseEntityDeletable<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityDeletable"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityDeletable()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities.
/// Implements <see cref="IEntityDeletable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityDeletable<TIdentity> : BaseEntityBase<TIdentity>, IEntityDeletable
    where TIdentity : IEquatable<TIdentity>;