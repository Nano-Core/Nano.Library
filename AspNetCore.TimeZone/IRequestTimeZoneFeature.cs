namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Represents the feature that provides the current request's timezone information.
    /// </summary>
    public interface IRequestTimeZoneFeature
    {
        /// <summary>
        /// The <see cref="RequestTimeZone"/> of the request.
        /// </summary>
        RequestTimeZone RequestTimeZone { get; }

        /// <summary>
        /// The <see cref="IRequestTimeZoneProvider"/> that determined the request's timezone information.
        /// If the value is <c>null</c> then no provider was used and the request's timezone was se to the value of <see cref="RequestTimeZoneOptions.DefaultRequestTimeZone"/>.
        /// </summary>
        IRequestTimeZoneProvider Provider { get; }
    }
}