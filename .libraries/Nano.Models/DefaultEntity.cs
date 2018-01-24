using System;

namespace Nano.Models
{
    /// <inheritdoc cref="BaseEntityIdentity{TIdentity}"/>
    public class DefaultEntity : BaseEntityIdentity<Guid>
    {
        /// <summary>
        /// Created At.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    }
}