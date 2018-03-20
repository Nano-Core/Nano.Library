using System;
using System.Globalization;
using Nano.Models.Extensions.Enums;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// DateTimeOffset Extensions.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Gets the week number.
        /// </summary>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The week number if the passe <see cref="DateTimeOffset"/>.</returns>
        public static int GetWeekNumber(this DateTimeOffset at)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var weekNumber = dfi.Calendar.GetWeekOfYear(at.DateTime, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            return weekNumber;
        }

        /// <summary>
        /// Get the start and end date of the day, matching the passed <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The start and end date.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetDayDates(this DateTimeOffset at)
        {
            var startAt = new DateTimeOffset(at.Year, at.Month, at.Day, 0, 0, 0, new TimeSpan());
            var endAt = startAt.AddDays(1).AddSeconds(-1);

            return (startAt, endAt);
        }

        /// <summary>
        /// Get the start and end date of the week, matching the passed <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The start and end date.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetWeekDates(this DateTimeOffset at)
        {
            var dfi = DateTimeFormatInfo.CurrentInfo;
            var startAt = at.AddDays(-(at.DayOfWeek - dfi.FirstDayOfWeek)).Subtract(at.TimeOfDay);
            var endAt = startAt.AddDays(7).AddSeconds(-1);

            return (startAt, endAt);
        }

        /// <summary>
        /// Gets the start and end date of the month, matching the passed <see cref="DateTimeOffset"/> instance.
        /// </summary>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The start and end dates.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetMonthDates(this DateTimeOffset at)
        {
            var startAt = new DateTimeOffset(at.Year, at.Month, 1, 0, 0, 0, new TimeSpan());
            var endAt = startAt.AddMonths(1).AddSeconds(-1);

            return (startAt, endAt);
        }

        /// <summary>
        /// Gets the start and end date of the year, matching the passed <see cref="DateTimeOffset"/> instance.
        /// </summary>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The start and end date.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetYearDates(this DateTimeOffset at)
        {
            var startAt = new DateTimeOffset(at.Year, 1, 1, 0, 0, 0, new TimeSpan());
            var endAt = startAt.AddYears(1).AddSeconds(-1);

            return (startAt, endAt);
        }

        /// <summary>
        /// Gets the start and end date of the <see cref="DateInterval"/> passed, matching the passed <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="dateInterval">The <see cref="DateInterval"/>.</param>
        /// <param name="at">The <see cref="DateTimeOffset"/>.</param>
        /// <returns>The start and end date.</returns>
        public static (DateTimeOffset, DateTimeOffset) GetDates(this DateTimeOffset at, DateInterval dateInterval)
        {
            switch (dateInterval)
            {
                case DateInterval.DAYLY:
                    return at.GetDayDates();

                case DateInterval.WEEKLY:
                    return at.GetWeekDates();

                case DateInterval.MONTHLY:
                    return at.GetMonthDates();

                case DateInterval.YEARLY:
                    return at.GetYearDates();

                default:
                    return (at.Date, at.Date);
            }
        }
    }
}