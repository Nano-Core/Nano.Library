using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public class DefaultEntity : DefaultEntity<Guid>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public DefaultEntity()
    {
        this.Id = Guid.NewGuid();
    }
}

/// <inheritdoc cref="IEntityWritable"/>
public class DefaultEntity<TIdentity> : BaseEntity<TIdentity>;