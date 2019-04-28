using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nano.Models;
using Nano.Security.Exceptions;
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

            var statusCode = response.StatusCode;
            var logeLevel = statusCode >= 500 && statusCode <= 599 
                ? LogLevel.Error 
                : LogLevel.Information;

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

                response.ContentType = request.ContentType;

                if (exception is UnauthorizedException)
                {
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var error = new Error(ex);
                    if (error.IsTranslated)
                        logeLevel = LogLevel.Information;

                    var result =
                        request.IsContentTypeJson()
                            ? JsonConvert.SerializeObject(error)
                            : request.IsContentTypeXml()
                                ? XmlConvert.SerializeObject(error)
                                : error.ToString();

                    await response
                        .WriteAsync(result);
                }
            }
            finally
            {
                var method = request.Method;
                var path = request.Path.Value;
                var id = httpContext.TraceIdentifier;
                var elapsed = (Stopwatch.GetTimestamp() - timestamp) * 1000D / Stopwatch.Frequency;
                var protocol = request.IsHttps ? request.Protocol.Replace("HTTP", "HTTPS") : request.Protocol;
                var queryString = request.QueryString.HasValue ? $"{request.QueryString.Value}" : string.Empty;
                
                this.Logger
                    .Log(logeLevel, exception, MESSAGE_TEMPLATE, protocol, method, path, queryString, statusCode, elapsed, id);
            }
        }
    }
}