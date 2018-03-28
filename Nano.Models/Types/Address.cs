using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// String.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public virtual string String { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        [Required]
        public virtual Location Location { get; set; } = new Location();
    }
}