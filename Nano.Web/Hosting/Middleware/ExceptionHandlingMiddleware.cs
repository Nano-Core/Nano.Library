using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nano.Security.Exceptions;
using Nano.Web.Const;
using Nano.Web.Hosting.Serialization;
using Nano.Web.Models;
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

            var logLevel = response.StatusCode >= 500 && response.StatusCode <= 599 
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
                    
                    logLevel = error.IsTranslated
                        ? LogLevel.Information
                        : LogLevel.Error;

                    var acceptHheader = request.Headers["Accept"];
                    var contentTypeHeader = request.Headers["Content-Type"];
                    var queryString = request.QueryString.HasValue 
                        ? request.QueryString.Value 
                        : string.Empty;

                    var result = acceptHheader.Any()
                        ? acceptHheader.Contains(HttpContentType.JSON)
                            ? JsonConvert.SerializeObject(error)
                            : acceptHheader.Contains(HttpContentType.XML)
                                ? XmlConvert.SerializeObject(error)
                                : error.ToString()
                        : contentTypeHeader.Any()
                            ? contentTypeHeader.Contains(HttpContentType.JSON)
                                ? JsonConvert.SerializeObject(error)
                                : contentTypeHeader.Contains(HttpContentType.XML)
                                    ? XmlConvert.SerializeObject(error)
                                    : error.ToString()
                            : queryString.Contains($"format={HttpContentType.JSON}")
                                ? JsonConvert.SerializeObject(error)
                                : queryString.Contains($"format={HttpContentType.XML}")
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
                    .Log(logLevel, exception, MESSAGE_TEMPLATE, protocol, method, path, queryString, response.StatusCode, elapsed, id);
            }
        }
    }
}