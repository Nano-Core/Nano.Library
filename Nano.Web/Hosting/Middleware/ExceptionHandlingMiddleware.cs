using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Models;
using Nano.Web.Hosting.Extensions;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class ExceptionHandlingMiddleware : IMiddleware
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

            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;

                if (response.HasStarted)
                    return;
                
                if (httpContext.Request.IsContentTypeHtml())
                    throw;

                const string SUMMARY = "Internal Server Error";
                var exceptions = new[] { ex.GetBaseException().Message };

                if (request.IsContentTypeHtml())
                {
                    response.Redirect($"home/error?summary={SUMMARY}&exceptions={exceptions}");
                }
                else
                {
                    var error = new Error { Summary = SUMMARY, Exceptions = exceptions };
                    var result = request.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(error)
                        : request.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(error)
                            : ex.Message;

                    await response
                        .WriteAsync(result);
                }
            }
        }
    }
}