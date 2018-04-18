using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Models
{
    /// <summary>
    /// An audit entry property.
    /// </summary>
    public class AuditEntryProperty
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the identifier of the audit entry property.
        /// </summary>
        public int AuditEntryPropertyID { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the audit entry.
        /// </summary>
        public int AuditEntryID { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public virtual DefaultAuditEntry Parent { get; set; }

        /// <summary>
        /// Gets or sets the name of the property audited.
        /// </summary>
        [MaxLength(255)]
        public virtual string PropertyName { get; set; }

        /// <summary
        /// >Gets or sets the name of the relation audited.
        /// </summary>
        [MaxLength(255)]
        public virtual string RelationName { get; set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        public virtual string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        public virtual string OldValue { get; set; }

        // ReSharper restore InconsistentNaming
    }
}