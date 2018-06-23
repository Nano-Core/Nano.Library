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
                headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept" });
                headers.Add("Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                response.StatusCode = (int)HttpStatusCode.OK;

                await response.WriteAsync("OK");
            }
            else
                await next.Invoke(httpContext);
        }
    }
}