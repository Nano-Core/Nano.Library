using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Attributes;
using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <summary>
    /// Base Entity User (abstract).
    /// </summary>
    public abstract class BaseEntityUser<TIdentity> : BaseEntityIdentity<TIdentity>, IEntityUser<TIdentity>, IEntityWritable
    {
        /// <inheritdoc />
        public virtual long IsDeleted { get; set; } = 0L;

        /// <inheritdoc />
        [MaxLength(128)]
        public virtual string IdentityUserId { get; set; }

        /// <inheritdoc />
        [Include]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}