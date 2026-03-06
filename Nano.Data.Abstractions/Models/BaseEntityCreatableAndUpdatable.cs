using System;
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
public abstract class BaseEntityCreatableAndUpdatable<TIdentity> : BaseEntityBase<TIdentity>, IEntityCreatable
    where TIdentity : IEquatable<TIdentity>;