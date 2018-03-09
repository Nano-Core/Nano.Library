using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Z.EntityFramework.Plus;

namespace Nano.Data.Models
{
    /// <summary>
    /// An audit entry.
    /// </summary>
    public class AuditEntry
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the identifier of the audit entry.
        /// </summary>
        public virtual int AuditEntryID { get; set; }

        /// <summary>
        /// Gets or sets who created this object.
        /// </summary>
        [MaxLength(255)]
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the the date of the changes.
        /// </summary>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity set.
        /// </summary>
        [MaxLength(255)]
        public virtual string EntitySetName { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity type.
        /// </summary>
        [MaxLength(255)]
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public virtual List<AuditEntryProperty> Properties { get; set; }

        /// <summary>
        /// Gets or sets the entry state.
        /// </summary>
        public virtual AuditEntryState State { get; set; }

        /// <summary>
        /// Gets or sets the name of the entry state.
        /// </summary>
        [MaxLength(255)]
        public virtual string StateName
        {
            get => State.ToString();
            set => State = (AuditEntryState)Enum.Parse(typeof(AuditEntryState), value);
        }

        // ReSharper restore InconsistentNaming
    }
}