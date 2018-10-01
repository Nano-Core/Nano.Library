using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Nano.Models.Extensions;

namespace Nano.Web.Hosting.Extensions
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
        public static string ReadBody(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            httpRequest.EnableRewind();

            try
            {
                return new StreamReader(httpRequest.Body).ReadToEnd();
            }
            finally
            {
                httpRequest.Body.Position = 0;
            }
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'application/json', 'application/javascript' or 
        /// the <see cref="HttpRequest.QueryString"/> contains a 'format=json' parameter.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeJson(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            return httpRequest.IsContentType(HttpContentType.JSON);
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'application/xml' or  
        /// the <see cref="HttpRequest.QueryString"/> contains a 'format=xml' parameter.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeXml(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            return httpRequest.IsContentType(HttpContentType.XML);
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest"/> headers contains a content-type of 'text/html' or
        /// the <see cref="HttpRequest.QueryString"/> contains a 'format=html' parameter.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentTypeHtml(this HttpRequest httpRequest)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            return httpRequest.IsContentType(HttpContentType.HTML);
        }

        /// <summary>
        /// Returns whether the <see cref="HttpRequest.ContentType"/> matches the passed <see cref="string"/>.
        /// </summary>
        /// <param name="httpRequest">The <see cref="HttpRequest"/>.</param>
        /// <param name="match">The <see cref="string"/> content-type.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public static bool IsContentType(this HttpRequest httpRequest, string match)
        {
            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            if (match == null)
                throw new ArgumentNullException(nameof(match));

            var headers = httpRequest.Headers;
            var queryString = httpRequest.QueryString.HasValue ? httpRequest.QueryString.Value : string.Empty;
            var accept = headers["Accept"].Join();
            var contentType = headers["Content-Type"].Join();

            match.TrySubstring("/", out var format);

            return accept.Contains(match) || contentType.Contains(match) || queryString.Contains($"format={format}");
        }
    }
}