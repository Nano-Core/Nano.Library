using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Hosting.Extensions;
using Nano.App.Hosting.Middleware.Interfaces;

namespace Nano.App.Hosting.Middleware
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