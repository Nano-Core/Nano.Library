using System;
using Nano.Models.Interfaces;

namespace Nano.Models;

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
public class DefaultEntity<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable
{
    /// <inheritdoc />
    public virtual long IsDeleted { get; set; } = 0L;

    /// <summary>
    /// Created At.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}