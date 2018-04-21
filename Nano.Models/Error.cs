using System.ComponentModel.DataAnnotations;

namespace Nano.Models
{
    /// <summary>
    /// Error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Message.
        /// </summary>
        [Required]
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string[] Exceptions { get; set; } = new string[0];
    }
}