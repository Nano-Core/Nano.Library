using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

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
        [JsonIgnore]
        [MaxLength(128)]
        public virtual string IdentityUserId { get; set; }

        /// <summary>
        /// Identity User.
        /// </summary>
        [JsonIgnore]
        public virtual IdentityUser IdentityUser { get; set; }
    }
}