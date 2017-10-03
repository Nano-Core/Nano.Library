using System;

namespace Nano.App.Models.Types
{
    /// <summary>
    /// Period.
    /// </summary>
    public class Period
    {
        /// <summary>
        /// Start.
        /// </summary>
        public virtual TimeSpan Start { get; set; }

        /// <summary>
        /// Finish.
        /// </summary>
        public virtual TimeSpan Finish { get; set; }

        /// <summary>
        /// Gets whether the <see cref="Period"/> is occuring or not, 
        /// by comparing the passed <see cref="TimeSpan"/> to <see cref="Period.Start"/> and <see cref="Period.Finish"/>.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/>.</param>
        /// <returns>Returns true if the <see cref="Period"/> is occuring, otherwise false.</returns>
        public virtual bool IsOccuring(TimeSpan timeSpan)
        {
            var isFound = timeSpan >= this.Start && timeSpan <= this.Finish;

            if (this.Start <= this.Finish)
                return isFound;

            if (timeSpan <= this.Start && timeSpan >= this.Finish)
            {
                isFound = false;
            }
            else if (timeSpan <= this.Start || timeSpan >= this.Finish)
            {
                isFound = true;
            }

            return isFound;
        }
    }
}