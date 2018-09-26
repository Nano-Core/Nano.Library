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
        /// Action.
        /// </summary>
        string Action { get; set; }

        /// <summary>
        /// Controller.
        /// </summary>
        string Controller { get; set; }

        /// <summary>
        /// Returns the Uri for the request.
        /// </summary>
        /// <param name="apiOptions">The <see cref="ApiOptions"/>.</param>
        /// <returns>The <see cref="Uri"/>.</returns>
        Uri GetUri(ApiOptions apiOptions);

        /// <summary>
        /// Get the collection  querystring  key/values.
        /// </summary>
        /// <returns>The <see cref="IList{KeyValuePair}"/>.</returns>
        IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}