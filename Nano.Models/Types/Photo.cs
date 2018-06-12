using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Photo.
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Url.
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public virtual string Url { get; set; }

        /// <summary>
        /// Reference.
        /// </summary>
        [MaxLength(128)]
        public virtual string Reference { get; set; }

        /// <summary>
        /// Alt.
        /// The alternative text shown if the image cannot be loaded, and when hovering the image.
        /// </summary>
        [MaxLength(256)]
        public virtual string Alt { get; set; }
    }
}