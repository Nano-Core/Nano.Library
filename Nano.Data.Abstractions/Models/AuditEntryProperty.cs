using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

// BUG: AUDIT: can we make AuditEntryProperty<Guid>? and it would work?
// The user needs to know the AuditEntry<TIdentity> (and the AuditEntryProperty<TIdentity>) to get them through api-client. Maybe this needs to be some place else, in Api docs??

/// <summary>
/// Represents a property-level audit entry for EF entities.
/// Implements <see cref="IEntityAuditableNegated"/>.
/// </summary>
public class AuditEntryProperty<TIdentity> : BaseEntity<TIdentity>, IEntityAuditableNegated
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identifier of the parent audit entry.
    /// </summary>
    [Required]
    public virtual Guid ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="AuditEntry{TIdentity}"/>.
    /// </summary>
    public virtual AuditEntry<TIdentity> Parent { get; set; } = null!;

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