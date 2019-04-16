using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Login Provider.
    /// </summary>
    public class LoginProvider
    {
        /// <summary>
        /// Name.
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Display Name.
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }
    }
}