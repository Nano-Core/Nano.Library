using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// An abstract base class provider for determining the timezone information
    /// of an <see cref="HttpRequest"/>.
    /// </summary>
    public abstract class RequestTimeZoneProvider : IRequestTimeZoneProvider
    {
        /// <summary>
        /// Result that indicates that this instance of <see cref="RequestTimeZoneProvider" />
        /// could not determine the request timezone.
        /// </summary>
        protected static readonly Task<ProviderTimeZoneResult> nullProviderTimeZoneResult = Task.FromResult(default(ProviderTimeZoneResult));

        /// <summary>
        /// The current options for the <see cref="RequestTimeZoneMiddleware"/>.
        /// </summary>
        public RequestTimeZoneOptions Options { get; set; }

        /// <inheritdoc />
        public abstract Task<ProviderTimeZoneResult> DetermineProviderTimeZoneResult(HttpContext httpContext);
    }
}