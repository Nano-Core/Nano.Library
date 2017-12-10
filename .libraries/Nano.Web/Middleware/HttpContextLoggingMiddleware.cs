using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.Web.Controllers.Extensions;
using Serilog;
using Serilog.Events;

namespace Nano.Web.Middleware
{
    /// <inheritdoc />
    public class HttpContextLoggingMiddleware : IMiddleware
    {
        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var timestamp = Stopwatch.GetTimestamp();

            Exception exception = default;
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
                var elapsed = (Stopwatch.GetTimestamp() - timestamp) * 1000D / Stopwatch.Frequency;
                var request = httpContext.Request;
                var response = httpContext.Response;
                var id = httpContext.TraceIdentifier;
                var ssl = request.IsHttps;
                var path = request.Path.Value;
                var host = request.Host.Value;
                var method = request.Method;
                var session = httpContext.Session;
                var protocol = request.Protocol;
                var queryString = request.QueryString.Value;
                var formData = request.HasFormContentType ? request.Form.ToDictionary(x => x.Key, x => x.Value.ToString()) : new Dictionary<string, string>();
                var requestHeaders = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
                var responseHeaders = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
                var statusCode = response.StatusCode;
                var logeLevel = statusCode >= 500 && statusCode <= 599 ? LogEventLevel.Error : LogEventLevel.Information;

                Log.Logger
                    .ForContext("Request", new
                    {
                        Id = id,
                        Ssl = ssl,
                        Path = path,
                        Host = host,
                        Method = method,
                        Form = formData,
                        Protocol = protocol,
                        Headers = requestHeaders,
                        QueryString = queryString,
                        Body = request.ReadBody()
                    }, true)
                    .ForContext("Response", new
                    {
                        Session = session,
                        Headers = responseHeaders
                    }, true)
                    .Write(logeLevel, exception, "{Message} {StatusCode} in {Elapsed:0.0000} ms.", $"{protocol} {method} {path}", statusCode, elapsed);
            }
        }
    }
}