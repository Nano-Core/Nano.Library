using System;
using Microsoft.AspNetCore.Builder;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Extension methods for adding the <see cref="RequestTimeZoneMiddleware"/> to an application.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="RequestTimeZoneMiddleware"/> to automatically set timezone information
        /// for requests based on information provided by the client.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRequestTimeZone(this IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            return applicationBuilder
                .UseMiddleware<RequestTimeZoneMiddleware>();
        }
    }
}