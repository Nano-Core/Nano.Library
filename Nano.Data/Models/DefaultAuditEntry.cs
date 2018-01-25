using System.ComponentModel.DataAnnotations;
using Nano.Models.Interfaces;
using Z.EntityFramework.Plus;

namespace Nano.Data.Models
{
    /// <summary>
    /// Default Audit Entry.
    /// </summary>
    public class DefaultAuditEntry : AuditEntry, IEntity
    {
        /// <summary>
        /// Request Id.
        /// </summary>
        [MaxLength(255)]
        public virtual string RequestId { get; set; }
    }
}