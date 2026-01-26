using System;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public class DefaultEntityUser : DefaultEntityUser<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultEntityUser"/> class with a new <see cref="Guid"/> identifier.
    /// </summary>
    protected DefaultEntityUser()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <inheritdoc />
public class DefaultEntityUser<TIdentity> : BaseEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;