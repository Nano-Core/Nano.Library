using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Web.Controllers.Extensions;

namespace Nano.App.Extensions.Middleware
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

            response.ContentType = request.IsContentTypeHtml()
                ? HttpContentType.HTML
                : request.IsContentTypeJson()
                    ? HttpContentType.JSON
                    : request.IsContentTypeXml()
                        ? HttpContentType.XML
                        : HttpContentType.TEXT;

            await next(httpContext);
        }
    }
}