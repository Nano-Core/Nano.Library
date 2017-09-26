using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Hosting.Middleware.Interfaces;
using Serilog.Events;

namespace Nano.App.Hosting.Middleware
{    
    /// <inheritdoc />
    public class HttpContextMiddleware : IHttpContextMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var start = Stopwatch.GetTimestamp();

            Exception exception = null;
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                HttpContextMiddleware.Log(httpContext, start, exception);
            }
        }

        private static void Log(HttpContext httpContext, long start, Exception exception = null)
        {
            var elapsed = (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency;

            const string MESSAGE_TEMPLATE = "{Protocol} {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms.";

            var request = httpContext.Request;
            var response = httpContext.Response;
            var ssl = request.IsHttps;
            var path = request.Path.Value;
            var host = request.Host.Value;
            var method = request.Method;
            var session = httpContext.Session;
            var protocol = request.Protocol;
            var statusCode = response.StatusCode;
            var queryString = request.QueryString.Value;
            var formData = request.HasFormContentType ? request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()) : new Dictionary<string, string>();
            var requestHeaders = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            var responseHeaders = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            var logLevel = exception == null ? LogEventLevel.Information : LogEventLevel.Error;

            var logger = Serilog.Log.Logger
                .ForContext("Request", new
                    {
                        Ssl = ssl,
                        Path = path,
                        Host = host,
                        Method = method,
                        Form = formData,
                        Protocol = protocol,
                        Headers = requestHeaders,
                        QueryString = queryString
                    }, true)
                .ForContext("Response", new
                    {
                        Session = session,
                        Headers = responseHeaders
                    }, true);

            if (exception == null)
                logger.Write(logLevel, MESSAGE_TEMPLATE, protocol, method, path, statusCode, elapsed);
            else
                logger.Write(logLevel, exception, MESSAGE_TEMPLATE, protocol, method, path, statusCode, elapsed);
        }
    }
}