using System;

namespace Nano.Models.Types
{
    /// <summary>
    /// Duration.
    /// </summary>
    public class Duration
    {
        /// <summary>
        /// Interval.
        /// </summary>
        public virtual TimeSpan Interval { get; set; }

        /// <summary>
        /// Adjustment.
        /// </summary>
        public virtual TimeSpan Adjustment { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public virtual TimeSpan Total => this.Interval.Add(this.Adjustment);
    }
}