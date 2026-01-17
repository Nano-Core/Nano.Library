using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc cref="IEntityIdentity{TIdentity}"/>
public abstract class BaseEntity<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable
{
    /// <inheritdoc />
    [Required]
    public virtual long IsDeleted { get; set; } = 0L;

    /// <summary>
    /// Created At.
    /// </summary>
    [Required]
    public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}