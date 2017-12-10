using System;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="BaseEntityUnique"/>
    public  class DefaultEntity : BaseEntityUnique, IEntityWritable
    {
        /// <summary>
        /// Is Active.
        /// Indicates whether the entity is active and relevant.
        /// NOTE: Only active instances are returned from queries, when filters are enabled (default behavior).
        /// </summary>
        public virtual bool IsActive { get; set; } = true;

        /// <summary>
        /// Created At.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Updated At.
        /// </summary>
        public virtual DateTimeOffset? UpdatedAt { get; protected set; }

        /// <summary>
        /// Expire At.
        /// Time when the instance will be scheduled for deletion.
        /// </summary>
        public virtual DateTimeOffset? ExpireAt { get; set; }
    }
}