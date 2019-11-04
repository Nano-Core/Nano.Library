using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nano.Models.Extensions;
using Nano.Web.Const;

namespace Nano.Web.Hosting.Extensions
{
    /// <summary>
    /// Http Request Extensions.
    /// </summary>
    public static class HttpRequestExtensions
    {
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
            var accept = headers["Accept"];
            var contentType = headers["Content-Type"];

            match.TrySubstring("/", out var format);

            return accept.Contains(match) || contentType.Contains(match) || queryString.Contains($"format={format}");
        }
    }
}