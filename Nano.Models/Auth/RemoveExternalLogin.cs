using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Auth
{
    /// <summary>
    /// Remove External Login.
    /// </summary>
    public class RemoveExternalLogin
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Required]
        public virtual string UserId { get; set; }

        /// <summary>
        /// Provider Key.
        /// </summary>
        [Required]
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// Login Provider.
        /// </summary>
        [Required]
        public virtual string LoginProvider { get; set; }
    }
}