using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nano.Models;
using Nano.Web.Hosting.Extensions;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        public ExceptionHandlingMiddleware(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.Logger = logger;
        }

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
                if (request.IsContentTypeHtml())
                    throw;

                if (response.HasStarted)
                    response.Clear();

                response.StatusCode = 500;
                response.ContentType = request.ContentType;

                var error = new Error
                {
                    Summary = "Internal Server Error",
                    Exceptions = new[] { ex.GetBaseException().Message }
                };

                var result = 
                    request.IsContentTypeJson()
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