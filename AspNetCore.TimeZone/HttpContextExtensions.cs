using System;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Http Context Extensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Get the <see cref="TimeZoneInfo"/>.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
        /// <returns>The token.</returns>
        public static TimeZoneInfo GetUserTimeZone(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            return httpContext.Features
                .Get<IRequestTimeZoneFeature>()
                .RequestTimeZone
                .TimeZone;
        }
    }
}