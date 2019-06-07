using System;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Details about the timezone obtained from <see cref="IRequestTimeZoneProvider"/>.
    /// </summary>
    public class ProviderTimeZoneResult
    {
        /// <summary>
        /// The timezone name.
        /// </summary>
        public virtual string TimeZoneName { get; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the timezone.</param>
        public ProviderTimeZoneResult(string name)
        {
            this.TimeZoneName = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}