using System;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// TimeSpan Extensions.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Is Within.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/>.</param>
        /// <param name="timeBegin">The <see cref="TimeSpan"/> begin.</param>
        /// <param name="timeEnd">The <see cref="TimeSpan"/> end.</param>
        /// <returns>A boolean whether the instance waas within begin and end.</returns>
        public static bool IsWithinPeriod(this TimeSpan timeSpan, TimeSpan timeBegin, TimeSpan timeEnd)
        {
            var isFound = timeSpan >= timeBegin && timeSpan <= timeEnd;
            if (timeBegin > timeEnd)
            {
                if (timeSpan <= timeBegin && timeSpan >= timeEnd)
                    isFound = false;
                else if (timeSpan <= timeBegin || timeSpan >= timeEnd)
                    isFound = true;
            }

            return isFound;
        }
    }
}