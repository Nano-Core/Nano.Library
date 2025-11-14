using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <summary>
/// Default Entity User.
/// </summary>
public class DefaultEntityUser : DefaultEntityUser<Guid>
{
    /// <inheritdoc />
    protected DefaultEntityUser()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <inheritdoc cref="IEntityWritable"/>
public class DefaultEntityUser<TIdentity> : BaseEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>;