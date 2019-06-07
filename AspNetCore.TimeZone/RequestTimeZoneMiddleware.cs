using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.TimeZone
{
    /// <inheritdoc />
    public class RequestTimeZoneMiddleware : IMiddleware
    {
        private readonly RequestTimeZoneOptions options;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="RequestTimeZoneOptions"/>.</param>
        public RequestTimeZoneMiddleware(RequestTimeZoneOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var requestTimeZone = this.options.DefaultRequestTimeZone;
            IRequestTimeZoneProvider winningProvider = null;

            if (this.options.RequestTimeZoneProviders != null)
            {
                foreach (var provider in this.options.RequestTimeZoneProviders)
                {
                    var providerTimeZoneResult = await provider
                        .DetermineProviderTimeZoneResult(httpContext);
                    
                    if (providerTimeZoneResult == null)
                        continue;

                    var result = new RequestTimeZone(providerTimeZoneResult.TimeZoneName);

                    if (result.TimeZone != null)
                    {
                        requestTimeZone = result;
                        winningProvider = provider;
                        break;
                    }
                }
            }

            httpContext.Features
                .Set<IRequestTimeZoneFeature>(new RequestTimeZoneFeature(requestTimeZone, winningProvider));

            httpContext.Response.Headers["TZ"] = requestTimeZone.TimeZone.Id;

            await next(httpContext);
        }
    }
}