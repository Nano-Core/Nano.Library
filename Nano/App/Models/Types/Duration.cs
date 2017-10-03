using System;

namespace Nano.App.Models.Types
{
    /// <summary>
    /// Duration.
    /// </summary>
    public class Duration
    {
        /// <summary>
        /// Time.
        /// </summary>
        public virtual TimeSpan Time { get; set; }

        /// <summary>
        /// Adjustment.
        /// </summary>
        public virtual TimeSpan Adjustment { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public virtual TimeSpan Total => this.Time.Add(this.Adjustment);
    }
}