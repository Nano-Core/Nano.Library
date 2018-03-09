using System;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// DateTime Extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the first and last day of the month of the passed <see cref="DateTime"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTime"/>.</param>
        /// <returns>The first and last day of the month of the passed <see cref="DateTime"/>.</returns>
        public static (DateTime, DateTime) GetMonthDays(this DateTime dateTime)
        {
            var firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return (firstDayOfMonth, lastDayOfMonth);
        }
    }
}