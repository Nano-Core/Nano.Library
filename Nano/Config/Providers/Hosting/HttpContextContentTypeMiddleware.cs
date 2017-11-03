using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Config.Providers.Hosting.Extensions;

namespace Nano.Config.Providers.Hosting
{
    /// <inheritdoc />
    public class HttpContextContentTypeMiddleware : IMiddleware
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
                ? HttpContentType.Html
                : request.IsContentTypeXml()
                    ? HttpContentType.Xml
                    : request.IsContentTypeJson()
                        ? HttpContentType.Json
                        : HttpContentType.Text;

            await next(httpContext);
        }
    }
}