using System;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc />
    public abstract class BaseEntity : IEntity
    {
        /// <inheritdoc />
        public virtual bool IsActive { get; set; } = true;

        /// <inheritdoc />
        public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        public virtual DateTimeOffset? UpdatedAt { get; set; }

        /// <inheritdoc />
        public virtual DateTimeOffset? ExpireAt { get; set; }
    }
}