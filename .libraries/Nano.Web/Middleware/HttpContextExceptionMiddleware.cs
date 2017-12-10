using System;
using System.ComponentModel.DataAnnotations;
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
                    // TODO: TEST: ModeState Validation
                    var a = ex as ValidationException;

                    Error error;
                    if (a != null)
                    {
                        error = new Error
                        {
                            Errors = (string[])a.Value
                        };
                    }
                    else
                    {
                        error = new Error
                        {
                            Errors = new[]
                            {
                                ex.Message
                            }
                        };
                    }

                    var textResult = response.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(error)
                        : response.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(error)
                            : response.IsContentTypeHtml()
                                ? default
                                : error.ToString();

                    await response
                        .WriteAsync(textResult);
                }
            }
        }
    }
}