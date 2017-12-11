using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Web.Controllers.Extensions;
using Newtonsoft.Json;

namespace Nano.Web.Middleware
{
    /// <inheritdoc />
    public class HttpContextExceptionMiddleware : IMiddleware
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
                await next(httpContext);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;

                if (!response.HasStarted)
                {
                    var error = new
                    {
                        Summary = "Internal Server Error",
                        Errors = new[] { ex.GetBaseException().Message }
                    };
                    
                    var textResult = response.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(error)
                        : response.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(error)
                            : response.IsContentTypeHtml()
                                ? default
                                : ex.Message;

                    await response
                        .WriteAsync(textResult);
                }
            }
        }
    }
}