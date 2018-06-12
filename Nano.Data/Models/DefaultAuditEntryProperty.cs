using System.ComponentModel.DataAnnotations;
using Nano.Models.Interfaces;

namespace Nano.Data.Models
{
    /// <summary>
    /// Default Audit Entry Property.
    /// </summary>
    public class DefaultAuditEntryProperty : IEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the audit entry property.
        /// </summary>
        public virtual int AuditEntryPropertyId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the audit entry.
        /// </summary>
        public virtual int AuditEntryId { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public virtual DefaultAuditEntry Parent { get; set; }

        /// <summary>
        /// Gets or sets the name of the property audited.
        /// </summary>
        [MaxLength(255)]
        public virtual string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the relation audited.
        /// </summary>
        [MaxLength(255)]
        public virtual string RelationName { get; set; }

        /// <summary>
        /// Gets or sets the new value audited formatted.
        /// </summary>
        public virtual string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value audited formatted.
        /// </summary>
        public virtual string OldValue { get; set; }
    }
}