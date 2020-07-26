using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Attributes;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc />
    public class BaseEntityUser : BaseEntityUser<Guid>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseEntityUser()
        {
            this.Id = Guid.NewGuid();
        }
    }

    /// <summary>
    /// Base Entity User (abstract).
    /// </summary>
    public abstract class BaseEntityUser<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityWritable, IEntityUser<TIdentity>
    {
        /// <inheritdoc />
        [MaxLength(128)]
        public virtual string IdentityUserId { get; set; }

        /// <inheritdoc />
        [Include]
        public virtual IdentityUser IdentityUser { get; set; }

        /// <inheritdoc />
        public virtual long IsDeleted { get; set; } = 0L;

        /// <summary>
        /// Created At.
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}