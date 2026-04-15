using Nano.Data.Abstractions.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Enums;

namespace Nano.Data.Abstractions.Models;

/// <inheritdoc />
public class AuditEntry : AuditEntry<Guid>;

/// <summary>
/// A class representing an audit entry.
/// </summary>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public class AuditEntry<TIdentity> : BaseEntity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Gets or sets the user who created the audited entity.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the Key of the audited entity.
    /// </summary>
    [Required]
    public virtual required TIdentity EntityKey { get; set; }

    /// <summary>
    /// Gets or sets the EF entity set name.
    /// </summary>
    [MaxLength(256)]
    public virtual string? EntitySetName { get; set; }

    /// <summary>
    /// Gets or sets the EF entity type name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string EntityTypeName { get; set; }

    /// <summary>
    /// Gets or sets the entity state as integer (Added, Modified, Deleted).
    /// </summary>
    [Required]
    [DefaultValue(0)]
    public virtual AuditState EntityState { get; set; } = AuditState.Added;

    /// <summary>
    /// Gets or sets the request identifier associated with this audit entry.
    /// </summary>
    [MaxLength(256)]
    public virtual string? RequestId { get; set; }

    /// <summary>
    /// Gets or sets the collection of property-level audit entries.
    /// </summary>
    [Required]
    [Include]
    public virtual ICollection<AuditEntryProperty<TIdentity>> Properties { get; set; } = [];
}