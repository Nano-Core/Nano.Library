using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nano.Models;
using Nano.Web.Hosting.Exceptions;
using Nano.Web.Hosting.Extensions;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;

namespace Nano.Web.Hosting.Middleware
{
    /// <inheritdoc />
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private const string MESSAGE_TEMPLATE = "{protocol} {method} {path}{queryString} {statusCode} in {elapsed:0.0000} ms. (Id={id})";

        /// <summary>
        /// Logger.
        /// </summary>
        protected virtual ILogger Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">the <see cref="ILogger"/></param>
        public ExceptionHandlingMiddleware(ILogger logger)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var timestamp = Stopwatch.GetTimestamp();

            var request = httpContext.Request;
            var response = httpContext.Response;

            Exception exception = default;
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                exception = ex.GetBaseException();

                if (response.HasStarted)
                    response.Clear();

                response.StatusCode = 500;
                response.ContentType = request.ContentType;

                var error = new Error
                {
                    Summary = "Internal Server Error",
                    Exceptions = new[] { exception.Message },
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    TranslationCode = ex is TranslationException translationException 
                        ? translationException.Code 
                        : -1
                };

                var result =
                    request.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(error)
                        : request.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(error)
                            : error.ToString();

                    await response
                        .WriteAsync(result);
            }
            finally
            {
                var method = request.Method;
                var path = request.Path.Value;
                var id = httpContext.TraceIdentifier;
                var statusCode = response.StatusCode;
                var elapsed = (Stopwatch.GetTimestamp() - timestamp) * 1000D / Stopwatch.Frequency;
                var protocol = request.IsHttps ? request.Protocol.Replace("HTTP", "HTTPS") : request.Protocol;
                var queryString = request.QueryString.HasValue ? $"{request.QueryString.Value}" : string.Empty;
                
                var logeLevel = statusCode >= 500 && statusCode <= 599 
                    ? LogLevel.Error 
                    : LogLevel.Information;

                this.Logger
                    .Log(logeLevel, exception, MESSAGE_TEMPLATE, protocol, method, path, queryString, statusCode, elapsed, id);
            }
        }
    }
}