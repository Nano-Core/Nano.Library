using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Determines the timezone information for a request via the value of the 'tz' header.
    /// </summary>
    public class HeaderRequestTimeZoneProvider : RequestTimeZoneProvider
    {
        /// <summary>
        /// The header key that contains the timezone name.
        /// </summary>
        public virtual string Headerkey { get; set; } = "tz";

        /// <inheritdoc />
        public override Task<ProviderTimeZoneResult> DetermineProviderTimeZoneResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var value = httpContext.Request
                .Headers[this.Headerkey];

            if (string.IsNullOrEmpty(value))
                return RequestTimeZoneProvider.nullProviderTimeZoneResult;

            var providerTimeZoneResult = new ProviderTimeZoneResult(value);

            return Task.FromResult(providerTimeZoneResult);
        }
    }
}