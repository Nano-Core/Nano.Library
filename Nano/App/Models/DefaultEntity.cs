using System;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models
{
    /// <inheritdoc cref="BaseEntityUnique"/>
    public abstract class DefaultEntity : BaseEntityUnique, IEntityWritable
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