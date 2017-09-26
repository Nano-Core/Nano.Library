using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Logging.Middleware.Interfaces;
using Serilog;

namespace Nano.App.Logging.Middleware
{
    /// <inheritdoc />
    public class HttpContextLoggingMiddleware : IHttpContextLoggingMiddleware
    {
        private static ILogger Logger => Log.ForContext<HttpContextLoggingMiddleware>();

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            const string MESSAGE_TEMPLATE = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            var request = httpContext.Request;
            var path = request.Path;
            var host = request.Host;
            var method = request.Method;
            var protocal = request.Protocol;
            var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
            var form = request.HasFormContentType ? request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()) : new Dictionary<string, string>();
            var queryString = request.QueryString;
            var statusCode = httpContext.Response?.StatusCode;

            var start = Stopwatch.GetTimestamp();
            try
            {
                await next(httpContext);

                if (statusCode >= 500)
                    throw new HttpRequestException();

                var elapsed = (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency;

                HttpContextLoggingMiddleware.Logger
                    .Information(MESSAGE_TEMPLATE, method, path, statusCode, elapsed);
            }
            catch (Exception ex)
            {
                var elapsed = (Stopwatch.GetTimestamp() - start) * 1000 / (double)Stopwatch.Frequency;

                HttpContextLoggingMiddleware.Logger
                    .ForContext("RequestHost", host)
                    .ForContext("RequestProtocol", protocal)
                    .ForContext("RequestHeaders", headers, true)
                    .ForContext("RequestForm", form, true)
                    .ForContext("RequestQueryString", queryString, true)
                    .Error(ex, MESSAGE_TEMPLATE, method, path, statusCode, elapsed);

                throw;
            }
        }
    }
}