using System;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Details about the timezone for an <see cref="HttpRequest"/>.
    /// </summary>
    public class RequestTimeZone
    {
        /// <summary>
        /// The <see cref="TimeZoneInfo"/>.
        /// </summary>
        public virtual TimeZoneInfo TimeZone { get; }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="name">The timezone name.</param>
        public RequestTimeZone(string name)
        {
            this.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(name);
        }
        
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="timeZoneInfo">The <see cref="TimeZoneInfo"/>.</param>
        public RequestTimeZone(TimeZoneInfo timeZoneInfo)
        {
            this.TimeZone = timeZoneInfo ?? throw new ArgumentNullException(nameof(timeZoneInfo));
        }
    }
}