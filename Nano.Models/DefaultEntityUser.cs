using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Attributes;

namespace Nano.Models
{
    /// <summary>
    /// Default Entity User.
    /// </summary>
    public class DefaultEntityUser : DefaultEntity
    {
        /// <summary>
        /// Identity User Id.
        /// </summary>
        [MaxLength(128)]
        public virtual string IdentityUserId { get; set; }

        /// <summary>
        /// Identity User.
        /// Always included using <see cref="IncludeAttribute"/>.
        /// </summary>
        [Include]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}