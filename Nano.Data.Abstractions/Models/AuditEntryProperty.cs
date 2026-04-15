using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public class AuditEntryProperty : AuditEntryProperty<Guid>;

/// <summary>
/// Represents a property-level audit entry for EF entities.
/// </summary>
public class AuditEntryProperty<TIdentity> : BaseEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the identifier of the parent audit entry.
    /// </summary>
    [Required]
    public virtual Guid ParentId { get; set; } = Guid.Empty;

    /// <summary>
    /// Gets or sets the parent <see cref="AuditEntry{TIdentity}"/>.
    /// </summary>
    public virtual AuditEntry<TIdentity> Parent { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the property audited.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets the relation name of the property audited.
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