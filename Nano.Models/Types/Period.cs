using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types
{
    /// <summary>
    /// Period.
    /// </summary>
    public class Period
    {
        /// <summary>
        /// Begin At.
        /// </summary>
        [Required]
        public virtual DateTimeOffset BeginAt { get; set; }

        /// <summary>
        /// Finish At.
        /// </summary>
        [Required]
        public virtual DateTimeOffset FinishAt { get; set; }

        /// <summary>
        /// Gets whether the <see cref="Period"/> is occuring or not, 
        /// by comparing the passed <see cref="DateTimeOffset"/> to <see cref="Period.BeginAt"/> and <see cref="Period.FinishAt"/>.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>Returns true if the <see cref="Period"/> is occuring, otherwise false.</returns>
        public virtual bool IsOccuring(DateTimeOffset dateTimeOffset)
        {
            var isFound = dateTimeOffset >= this.BeginAt && dateTimeOffset <= this.FinishAt;

            if (this.BeginAt <= this.FinishAt)
                return isFound;

            if (dateTimeOffset <= this.BeginAt && dateTimeOffset >= this.FinishAt)
            {
                isFound = false;
            }
            else if (dateTimeOffset <= this.BeginAt || dateTimeOffset >= this.FinishAt)
            {
                isFound = true;
            }

            return isFound;
        }
    }
}