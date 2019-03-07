using System;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc cref="IEntityWritable"/>
    public class DefaultEntity : BaseEntityIdentity<Guid>, IEntityWritable
    {
        /// <inheritdoc />
        public virtual long IsDeleted { get; set; } = 0L;

        /// <summary>
        /// Created At.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultEntity()
        {
            this.Id = Guid.NewGuid();
        }
    }
}