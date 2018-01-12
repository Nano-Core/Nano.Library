using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Nano.Web.Middleware
{
    /// <inheritdoc />
    public class HttpRequestIdentifierMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var identifier = httpContext.Features.Get<IHttpRequestIdentifierFeature>();

            if (identifier?.TraceIdentifier != null)
                httpContext.Response.Headers["RequestId"] = identifier.TraceIdentifier;

            await next(httpContext);
        }
    }
}