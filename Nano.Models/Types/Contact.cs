using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Contact.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Name.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Phone Number.
        /// </summary>
        [Required]
        public virtual PhoneNumber PhoneNumber { get; set; } = new PhoneNumber();

        /// <summary>
        /// Admin Email Address.
        /// </summary>
        [Required]
        public virtual EmailAddress EmailAddress { get; set; } = new EmailAddress();
    }
}