using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Duration.
    /// </summary>
    public class Duration
    {
        /// <summary>
        /// Time.
        /// </summary>
        [Required]
        public virtual TimeSpan Time { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Adjustment.
        /// </summary>
        [Required]
        public virtual TimeSpan Adjustment { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Total.
        /// </summary>
        [Required]
        public virtual TimeSpan Total
        {
            get => this.Time.Add(this.Adjustment);
            protected set
            {
                // do nothing.
            }
        }
    }
}