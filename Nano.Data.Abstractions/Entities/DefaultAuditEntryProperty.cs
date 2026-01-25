using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Abstractions.Entities;

/// <summary>
/// Represents a property-level audit entry for EF entities.
/// Implements <see cref="IEntityAuditableNegated"/>.
/// </summary>
public class DefaultAuditEntryProperty : DefaultEntity, IEntityAuditableNegated
{
    /// <summary>
    /// Gets or sets the identifier of the parent audit entry.
    /// </summary>
    [Required]
    public virtual Guid ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DefaultAuditEntry"/>.
    /// </summary>
    public virtual DefaultAuditEntry Parent { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the property audited.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string PropertyName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the related property audited (if applicable).
    /// </summary>
    [MaxLength(256)]
    public virtual string? RelationName { get; set; }

    /// <summary>
    /// Gets or sets the new value of the property formatted as string.
    /// </summary>
    public virtual string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the old value of the property formatted as string.
    /// </summary>
    public virtual string? OldValue { get; set; }
}