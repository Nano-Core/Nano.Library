using System;

namespace Nano.Models
{
    /// <inheritdoc />
    public class DefaultEntity : BaseEntityIdentity<Guid>
    {
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