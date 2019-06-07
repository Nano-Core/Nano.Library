using System;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Provides the current request's timezone information.
    /// </summary>
    public class RequestTimeZoneFeature : IRequestTimeZoneFeature
    {
        /// <inheritdoc />
        public virtual RequestTimeZone RequestTimeZone { get; }

        /// <inheritdoc />
        public virtual IRequestTimeZoneProvider Provider { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="requestTimeZone">The <see cref="RequestTimeZone"/>.</param>
        /// <param name="provider">The <see cref="IRequestTimeZoneProvider"/>.</param>
        public RequestTimeZoneFeature(RequestTimeZone requestTimeZone, IRequestTimeZoneProvider provider = null)
        {
            this.RequestTimeZone = requestTimeZone ?? throw new ArgumentNullException(nameof(requestTimeZone));
            this.Provider = provider;
        }
    }
}