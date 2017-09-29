using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Internal;

namespace Nano.App.Hosting.Extensions
{
    /// <summary>
    /// Http Request Extensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets the <see cref="HttpRequest.Body"/> as a raw string.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="string"/> read from the <see cref="Stream"/> of <see cref="HttpRequest.Body"/>.</returns>
        public static string GetBodyRewinded(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            httpRequest.EnableRewind();

            try
            {
                return new StreamReader(httpRequest.Body).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                httpRequest.Body.Position = 0;
            }
        }

        /// <summary>
        /// Gets the content-type matching the request header 'Accpt' or 'Content-Type'.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="string"/> matching the content-type of the request header.</returns>
        public static string GetContentType(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

           return httpRequest.IsContentTypeHtml()
                ? "text/html"
                : httpRequest.IsContentTypeXml()
                    ? "application/xml"
                    : httpRequest.IsContentTypeJson()
                        ? "application/json"
                        : "text/plain";
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'application/json' or 'application/javascript'. 
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeJson(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            var accept = httpRequest.Headers["Accept"].Join();
            var contentType = httpRequest.Headers["Content-Type"].Join();

            return accept.Contains("application/json") || contentType.Contains("application/json") || httpRequest.QueryString.Value.Contains("format=json");
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'application/xml'. 
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeXml(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            var accept = httpRequest.Headers["Accept"].Join();
            var contentType = httpRequest.Headers["Content-Type"];

            return accept.Contains("application/xml") || contentType.Contains("application/xml") || httpRequest.QueryString.Value.Contains("format=xml");
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'text/html'. 
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeHtml(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            var accept = httpRequest.Headers["Accept"].Join();
            var contentType = httpRequest.Headers["Content-Type"].Join();

            return accept.Contains("text/html") || contentType.Contains("text/html") || httpRequest.QueryString.Value.Contains("format=html");
        }
    }
}