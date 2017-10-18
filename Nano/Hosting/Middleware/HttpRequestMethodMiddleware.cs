using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Hosting.Middleware.Interfaces;

namespace Nano.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpRequestMethodMiddleware : IHttpRequestIdentifierMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if (HttpMethods.IsOptions(httpContext.Request.Method))
                await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
 
            await next(httpContext);
        }
    }
}