using System;
using Nano.Models.Interfaces;

namespace Nano.Models.Data;

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
public class DefaultEntity<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable;