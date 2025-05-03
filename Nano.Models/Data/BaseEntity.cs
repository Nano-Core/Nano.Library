using System;
using Nano.Models.Interfaces;

namespace Nano.Models.Data;

/// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
public abstract class BaseEntity<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable
{
    /// <inheritdoc />
    public virtual long IsDeleted { get; set; } = 0L;

    /// <summary>
    /// Created At.
    /// </summary>
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}