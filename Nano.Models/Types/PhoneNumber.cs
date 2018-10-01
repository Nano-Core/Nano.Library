using System.ComponentModel.DataAnnotations;

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
        [Phone]
        [MaxLength(20)]
        public virtual string Number { get; set; }
    }
}