using System;
using System.Globalization;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// DateTimeOffset Extensions.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Gets the first and last day of the month of the passed <see cref="DateTimeOffset"/> instance.
        /// </summary>
        /// <param name="dateTime">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The first and last day of the month of the passed <see cref="DateTimeOffset"/>.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetMonthDays(this DateTimeOffset dateTime)
        {
            var firstDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return (firstDayOfMonth, lastDayOfMonth);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetWeekNumber(this DateTimeOffset dateTime)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var weekOfYear = dfi.Calendar.GetWeekOfYear(dateTime.DateTime, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            return weekOfYear;
        }
    }
}