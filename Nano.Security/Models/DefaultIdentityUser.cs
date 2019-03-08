using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Nano.Models;
using Nano.Models.Types;
using Newtonsoft.Json;

namespace Nano.Security.Models
{
    /// <summary>
    /// Default Identity User.
    /// </summary>
    public class DefaultIdentityUser : DefaultEntity
    {
        /// <summary>
        /// Identity User Id.
        /// </summary>
        [MaxLength(128)]
        public virtual string IdentityUserId { get; set; }

        /// <summary>
        /// Identity User.
        /// </summary>
        [JsonIgnore]
        public virtual IdentityUser IdentityUser { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Password { get; set; }
        
        /// <summary>
        /// Email Address.
        /// </summary>
        [Required]
        public virtual EmailAddress EmailAddress { get; set; } = new EmailAddress();
    }
}