using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Hosting.Middleware.Extensions;
using Nano.Hosting.Middleware.Interfaces;

namespace Nano.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpRequestContentTypeMiddleware : IHttpRequestContentTypeMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            httpContext.Response.ContentType = httpContext.Request.GetContentType();

            await next(httpContext);
        }
    }
}