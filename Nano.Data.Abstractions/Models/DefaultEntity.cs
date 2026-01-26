using System;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public class DefaultEntity : DefaultEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultEntity"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    public DefaultEntity()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <inheritdoc />
public class DefaultEntity<TIdentity> : BaseEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>;