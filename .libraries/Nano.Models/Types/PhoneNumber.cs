using System.ComponentModel.DataAnnotations;
using Nano.Models.Attributes;

namespace Nano.Models.Types
{
    /// <summary>
    /// Phone Number
    /// </summary>
    public class PhoneNumber
    {
        /// <summary>
        /// Number (E164).
        /// </summary>
        [Required]
        [PhoneNumber]
        [MaxLength(20)]
        public virtual string Number { get; set; }
    }
}