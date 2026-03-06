using Nano.Data.Abstractions.Models.Abstractions;
using System;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityUpdatable : BaseEntityUpdatable<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntityUpdatable"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityUpdatable()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities.
/// Implements <see cref="IEntityUpdatable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityUpdatable<TIdentity> : BaseEntityBase<TIdentity>, IEntityUpdatable
    where TIdentity : IEquatable<TIdentity>;