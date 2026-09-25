using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityCreatableAndDeletable : BaseEntityCreatableAndDeletable<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityCreatableAndDeletable"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityCreatableAndDeletable()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities that are only ever created and deleted, never updated - such as a join row between two entities.
/// Implements <see cref="IEntityCreatable"/> and <see cref="IEntityDeletable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityCreatableAndDeletable<TIdentity> : BaseEntityReadOnly<TIdentity>, IEntityCreatable, IEntityDeletable
    where TIdentity : IEquatable<TIdentity>;
