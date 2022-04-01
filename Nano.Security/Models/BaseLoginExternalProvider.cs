using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models
{
    /// <summary>
    /// Base Login External Provider (abstract).
    /// </summary>
    public abstract class BaseLoginExternalProvider
    {
        /// <summary>
        /// Name.
        /// </summary>
        [Required]
        public virtual string Name { get; set; }
    }
}