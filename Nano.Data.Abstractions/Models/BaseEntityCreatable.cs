using Nano.Data.Abstractions.Models.Abstractions;
using System;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public abstract class BaseEntityCreatable : BaseEntityCreatable<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected BaseEntityCreatable()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <summary>
/// Base class for entities.
/// Implements <see cref="IEntityCreatable"/>.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identity.</typeparam>
public abstract class BaseEntityCreatable<TIdentity> : BaseEntityBase<TIdentity>, IEntityCreatable
    where TIdentity : IEquatable<TIdentity>;