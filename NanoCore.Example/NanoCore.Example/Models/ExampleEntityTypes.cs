using System.ComponentModel.DataAnnotations;
using Nano.Models;
using Nano.Models.Types;
using NanoCore.Example.Models.Types;

namespace NanoCore.Example.Models
{
    /// <summary>
    /// Example Entity Types.
    /// </summary>
    public class ExampleEntityTypes : DefaultEntity
    {
        /// <summary>
        /// Angle.
        /// </summary>
        public virtual Angle Angle { get; set; }

        /// <summary>
        /// Authentication Credential.
        /// </summary>
        public virtual AuthenticationCredential AuthenticationCredential { get; set; }

        /// <summary>
        /// Duration.
        /// </summary>
        public virtual Duration Duration { get; set; }

        /// <summary>
        /// Distance.
        /// </summary>
        public virtual Distance Distance { get; set; }

        /// <summary>
        /// Distance.
        /// </summary>
        public virtual EmailAddress EmailAddress { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>
        /// Percentage.
        /// </summary>
        public virtual Percentage Percentage { get; set; }

        /// <summary>
        /// Period.
        /// </summary>
        public virtual Period Period { get; set; }

        /// <summary>
        /// Phone Number.
        /// </summary>
        public virtual PhoneNumber PhoneNumber { get; set; }

        /// <summary>
        /// Nested.
        /// </summary>
        [Required]
        public virtual NestedType Nested { get; set; } = new NestedType();
    }
}