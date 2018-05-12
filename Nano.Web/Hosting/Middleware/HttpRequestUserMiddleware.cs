using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Security.Extensions;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpRequestUserMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            httpContext.Response.Headers["User"] = httpContext.Request.GetUser();

            await next(httpContext);
        }
    }
}