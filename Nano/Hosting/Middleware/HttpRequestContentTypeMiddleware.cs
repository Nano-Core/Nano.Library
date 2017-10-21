using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Controllers.Criteria.Entities;
using Nano.Common.Serialization;
using Nano.Hosting.Constants;
using Nano.Hosting.Extensions;
using Newtonsoft.Json;

namespace Nano.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpRequestContentTypeMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var response = httpContext.Response;

            try
            {
                httpContext.Response.ContentType = httpContext.Request.IsContentTypeHtml()
                    ? HttpContentType.Html
                    : httpContext.Request.IsContentTypeXml()
                        ? HttpContentType.Xml
                        : httpContext.Request.IsContentTypeJson()
                            ? HttpContentType.Json
                            : HttpContentType.Text;

                await next(httpContext);
            }
            catch
            {
                response.StatusCode = 500;
            }
            finally
            {
                if (!response.HasStarted)
                {
                    var result = new Error(response.StatusCode);
                    var textResult = response.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(result)
                        : response.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(result)
                            : response.IsContentTypeHtml()
                                ? default
                                : result.ToString();

                    await response
                        .WriteAsync(textResult);
                }
            }
        }
    }
}