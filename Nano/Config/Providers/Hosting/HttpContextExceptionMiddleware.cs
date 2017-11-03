using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Config.Providers.Hosting.Extensions;
using Nano.Models;
using Nano.Models.Types;
using Newtonsoft.Json;

namespace Nano.Config.Providers.Hosting
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
                    var error = new Error(response.StatusCode)
                    {
                        Errors = new[] { ex.GetBaseException().Message }
                    };

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