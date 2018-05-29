using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Range.
    /// A lower and upper limit.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// Lower.
        /// The lower limit of the range.
        /// </summary>
        [Required]
        [DefaultValue(0)]
        public virtual int Lower { get; set; } = 0;

        /// <summary>
        /// Upper.
        /// The upper limit of the range.
        /// </summary>
        public virtual int? Upper { get; set; }
    }
}