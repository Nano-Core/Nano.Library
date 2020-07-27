using System;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <summary>
    /// Default Entity User.
    /// </summary>
    public class DefaultEntityUser : DefaultEntityUser<Guid>
    {
        /// <inheritdoc />
        protected DefaultEntityUser()
        {
            this.Id = Guid.NewGuid();
        }
    }

    /// <inheritdoc cref="IEntityWritable"/>
    public class DefaultEntityUser<TIdentity> : BaseEntityUser<TIdentity>
    {
        /// <summary>
        /// Created At.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}