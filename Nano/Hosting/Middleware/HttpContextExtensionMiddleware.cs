using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nano.App.Config;
using Nano.App.Controllers.Contracts;
using Nano.Hosting.Extensions;
using Nano.Hosting.Middleware.Extensions;
using Nano.Hosting.Middleware.Interfaces;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace Nano.Hosting.Middleware
{
    /// <inheritdoc />
    public class HttpContextExtensionMiddleware : IHttpContextExtensionMiddleware
    {
        /// <summary>
        /// App Options.
        /// </summary>
        protected AppOptions AppOptions { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="appOptions">The <see cref="AppOptions"/>.</param>
        public HttpContextExtensionMiddleware(AppOptions appOptions)
        {
            if (appOptions == null)
                throw new ArgumentNullException(nameof(appOptions));

            this.AppOptions = appOptions;
        }

        /// <inheritdoc />
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var time = Stopwatch.GetTimestamp();
            var request = httpContext.Request;
            var response = httpContext.Response;

            Exception exception = null;
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                exception = ex;

                response.StatusCode = 500;
            }
            finally
            {
                if (!response.HasStarted)
                {
                    var result = new Error(response.StatusCode);
                    var textResult = response.IsContentTypeJson()
                        ? JsonConvert.SerializeObject(result)
                        : response.IsContentTypeXml()
                            ? XmlConvert.SerializeObject(result)
                            : response.IsContentTypeHtml()
                                ? null
                                : result.ToString();

                    if (textResult != null)
                        await response.WriteAsync(textResult);
                    else
                        response.Redirect($"/api/{this.AppOptions.Name}/home/error/{result.StatusCode}");
                }

                const string MESSAGE_TEMPLATE = "{Message} {StatusCode} in {Elapsed:0.0000} ms.";

                var elapsed = (Stopwatch.GetTimestamp() - time) * 1000D / Stopwatch.Frequency;
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
                var statusCode = response.StatusCode;
                var logeLevel = statusCode >= 500 && statusCode <= 599 ? LogEventLevel.Error : LogEventLevel.Information;
                var responseHeaders = response.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
                var message = $"{protocol} {method} {path}";

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
                        Body = request.GetBodyRewinded()
                    }, true)
                    .ForContext("Response", new
                    {
                        Session = session,
                        Headers = responseHeaders
                    }, true)
                    .Write(logeLevel, exception, MESSAGE_TEMPLATE, message, statusCode, elapsed);
            }
        }
    }
}