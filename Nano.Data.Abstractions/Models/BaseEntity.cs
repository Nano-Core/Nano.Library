using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

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