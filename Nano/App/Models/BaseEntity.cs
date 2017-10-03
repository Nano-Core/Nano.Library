using System;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models
{
    /// <inheritdoc />
    public abstract class BaseEntity : IEntity
    {
        /// <inheritdoc />
        public virtual bool IsActive { get; set; } = true;

        /// <inheritdoc />
        public virtual DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;

        /// <inheritdoc />
        public virtual DateTimeOffset? UpdatedAt { get; protected set; }

        /// <inheritdoc />
        public virtual DateTimeOffset? ExpireAt { get; set; }
    }
}