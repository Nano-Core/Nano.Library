using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Nano.Models.Data;
using Nano.Models.Interfaces;

namespace Nano.Data.Models;

/// <summary>
/// Default Audit Entry.
/// </summary>
public class DefaultAuditEntry : DefaultEntity, IEntityAuditableNegated
{
    /// <summary>
    /// Created By.
    /// </summary>
    [MaxLength(255)]
    public virtual string CreatedBy { get; set; }

    /// <summary>
    /// Entity Set Name.
    /// </summary>
    [MaxLength(255)]
    public virtual string EntitySetName { get; set; }

    /// <summary>
    /// Entity Type Name.
    /// </summary>
    [MaxLength(255)]
    public virtual string EntityTypeName { get; set; }

    /// <summary>
    /// State.
    /// </summary>
    public virtual int State { get; set; }

    /// <summary>
    /// State Name.
    /// </summary>
    [MaxLength(255)]
    public virtual string StateName { get; set; }

    /// <summary>
    /// Request Id.
    /// </summary>
    [MaxLength(255)]
    public virtual string RequestId { get; set; }

    /// <summary>
    /// Gets or sets the properties.
    /// </summary>
    public virtual ICollection<DefaultAuditEntryProperty> Properties { get; set; } = new Collection<DefaultAuditEntryProperty>();
}