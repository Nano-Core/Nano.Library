using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Nano.App.Hosting.Extensions
{
    /// <summary>
    /// Http Response Extensions.
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Gets the content-type matching the response header 'Accpt' or 'Content-Type'.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <returns>A <see cref="string"/> matching the content-type of the response header.</returns>
        public static string GetContentType(this HttpResponse httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            return httpResponse.IsContentTypeHtml()
                ? "text/html"
                : httpResponse.IsContentTypeXml()
                    ? "application/xml"
                    : httpResponse.IsContentTypeJson()
                        ? "application/json"
                        : "text/plain";
        }

        /// <summary>
        /// Returns whether the <see cref="HttpResponse"/> headers contains a content-type of 'application/json'. 
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeJson(this HttpResponse httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            var contentType = httpResponse.Headers["Content-Type"];

            return contentType.Contains("application/json");
        }

        /// <summary>
        /// Returns whether the <see cref="HttpResponse"/> headers contains a content-type of 'application/xml'. 
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeXml(this HttpResponse httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            var contentType = httpResponse.Headers["Content-Type"];

            return contentType.Contains("application/xml");
        }

        /// <summary>
        /// Returns whether the <see cref="HttpResponse"/> headers contains a content-type of 'text/html'. 
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeHtml(this HttpResponse httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            var contentType = httpResponse.Headers["Content-Type"];

            return contentType.Contains("text/html");
        }
    }
}