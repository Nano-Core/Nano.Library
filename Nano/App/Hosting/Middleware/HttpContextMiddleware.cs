using Microsoft.AspNetCore.Http;
using Nano.App.Hosting.Extensions;
using Nano.App.Hosting.Middleware.Interfaces;
using Nano.Controllers.Contracts;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
                    var result = new ErrorResult(response.StatusCode);

                    if (response.IsContentTypeJson())
                    {
                        await response.WriteAsync(JsonConvert.SerializeObject(result));
                    }
                    else if (response.IsContentTypeXml())
                    {
                        await response.WriteAsync(XmlConvert.SerializeObject(result));
                    }
                    else if (response.IsContentTypeHtml())
                    {
                        response.Redirect($"/api/globale/home/error/{result.StatusCode}");
                    }
                    else
                    {
                        await response.WriteAsync(result.ToString());
                    }
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

    /// <summary>
    /// 
    /// </summary>
    public static class XmlConvert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="defaultNamespace"></param>
        /// <param name="namespaces"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T obj, string defaultNamespace = null, XmlSerializerNamespaces namespaces = null, Type[] knownTypes = null)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), null, knownTypes, null, defaultNamespace);

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj, namespaces);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="defaultNamespace"></param>
        /// <param name="knownTypes"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string xml, string defaultNamespace = null, Type[] knownTypes = null)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), null, knownTypes, null, defaultNamespace);

            using (var stringReader = new StringReader(xml))
            {
                return (T)xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}