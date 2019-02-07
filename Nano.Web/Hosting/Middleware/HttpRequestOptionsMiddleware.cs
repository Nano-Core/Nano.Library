using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpRequestOptionsMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if (httpContext.Request.Method == HttpMethods.Options)
            {
                var response = httpContext.Response;
                var headers = response.Headers;

                headers.Add("Access-Control-Allow-Origin", new[] { (string)httpContext.Request.Headers["Origin"] });
                headers.Add("Access-Control-Allow-Headers", new[] { "Origin, Server, Date, Cache-Control, Accept, Content-Type, Transfer-Encoding, Connection, Content-Encoding, RequestId, api-supported-versions, Strict-Transport-Security, X-Frame-Options, X-XSS-Protection, X-Content-Type-Options, X-Download-Options, X-Robots-Tag, X-Requested-With" });
                headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, PATCH, DELETE, OPTIONS" });
                headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                response.StatusCode = (int)HttpStatusCode.OK;

                await response.WriteAsync("OK");
            }
            else
                await next.Invoke(httpContext);
        }
    }
}