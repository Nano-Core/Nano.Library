using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Nano.Hosting.Constants;

namespace Nano.Hosting.Extensions
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
                ? HttpContentType.Html
                : httpResponse.IsContentTypeXml()
                    ? HttpContentType.Xml
                    : httpResponse.IsContentTypeJson()
                        ? HttpContentType.Json
                        : HttpContentType.Text;
        }

        /// <summary>
        /// Returns whether the <see cref="HttpResponse"/> headers contains a content-type of 'application/json' or 'application/javascript'.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeJson(this HttpResponse httpResponse)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            return httpResponse.IsContentType(HttpContentType.Json);
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

            return httpResponse.IsContentType(HttpContentType.Xml);
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

            return httpResponse.IsContentType(HttpContentType.Html);
        }

        /// <summary>
        /// Returns whether the <see cref="HttpResponse.ContentType"/> matches the passed <see cref="string"/>.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/>.</param>
        /// <param name="match">The <see cref="string"/> content-type.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentType(this HttpResponse httpResponse, string match)
        {
            if (httpResponse == null)
                throw new ArgumentNullException(nameof(httpResponse));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            var headers = httpResponse.Headers;
            var contentType = headers["Content-Type"].Join();

            return contentType.Contains(match);
        }
    }
}