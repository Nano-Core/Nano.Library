using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Models;
using Nano.Web.Controllers.Extensions;
using Newtonsoft.Json;

namespace Nano.App.Extensions.Middleware
{
    /// <inheritdoc />
    public class HttpExceptionHandlingMiddleware : IMiddleware
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

                if (response.HasStarted)
                    return;
                
                if (httpContext.Response.IsContentTypeHtml())
                    throw;

                var error = new Error
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
                    .WriteAsync(textResult ?? string.Empty);
            }
        }
    }
}