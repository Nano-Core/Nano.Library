using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Web.Extensions;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpContentTypeMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var request = httpContext.Request;
            var response = httpContext.Response;

            response.ContentType = request.IsContentTypeJson()
                ? HttpContentType.JSON
                : request.IsContentTypeXml()
                    ? HttpContentType.XML
                    : request.IsContentTypeText()
                        ? HttpContentType.TEXT
                        : HttpContentType.HTML;

            await next(httpContext);
        }
    }
}