using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions.Models;

// BUG: can we make AuditEntry<Guid>? and it would work?

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class AuditEntry<TIdentity> : BaseEntity<TIdentity>, IEntityAuditableNegated
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the user who created the audited entity.
    /// </summary>
    [MaxLength(256)]
    public virtual string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the EF entity set name.
    /// </summary>
    [MaxLength(256)]
    public virtual string? EntitySetName { get; set; }

    /// <summary>
    /// Gets or sets the EF entity type name.
    /// </summary>
    [MaxLength(256)]
    public virtual string? EntityTypeName { get; set; }

    /// <summary>
    /// Gets or sets the entity state as integer (Added, Modified, Deleted).
    /// </summary>
    public virtual int State { get; set; }

    /// <summary>
    /// Gets or sets the name of the entity state (Added, Modified, Deleted).
    /// </summary>
    [MaxLength(256)]
    public virtual string? StateName { get; set; }

    /// <summary>
    /// Gets or sets the request identifier associated with this audit entry.
    /// </summary>
    [MaxLength(256)]
    public virtual string? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the collection of property-level audit entries.
    /// </summary>
    [Include]
    public virtual ICollection<AuditEntryProperty<TIdentity>> Properties { get; set; } = [];
}