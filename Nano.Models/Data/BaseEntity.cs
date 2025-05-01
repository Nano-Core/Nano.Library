using Nano.Models.Interfaces;
using System;

namespace Nano.Models.Data;

/// <inheritdoc />
public abstract class BaseEntity : IEntity
{
    /// <summary>
    /// Is Deleted.
    /// </summary>
    public virtual long IsDeleted { get; set; } = 0L;

    /// <inheritdoc />
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}