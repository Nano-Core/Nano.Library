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
        public virtual TimeSpan Time { get; set; }

        /// <summary>
        /// Adjustment.
        /// </summary>
        public virtual TimeSpan Adjustment { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public virtual TimeSpan Total
        {
            get => this.Time.Add(this.Adjustment);
            protected set { }
        }
    }
}