using System;
using System.Collections.Generic;

namespace Nano.Web.Api.Requests.Interfaces
{
    /// <summary>
    /// Base interface for requests.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Returns the Uri for the request.
        /// </summary>
        /// <typeparam name="TResponse">The type of response.</typeparam>
        /// <param name="apiOptions">The <see cref="ApiOptions"/>.</param>
        /// <returns>The <see cref="Uri"/>.</returns>
        Uri GetUri<TResponse>(ApiOptions apiOptions);

        /// <summary>
        /// Get the collection  querystring  key/values.
        /// </summary>
        /// <returns>The <see cref="IList{KeyValuePair}"/>.</returns>
        IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}