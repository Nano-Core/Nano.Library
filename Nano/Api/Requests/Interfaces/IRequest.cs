using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nano.Api.Requests.Interfaces
{
    /// <summary>
    /// Request interface.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Context.
        /// </summary>
        [JsonIgnore]
        ApiConnect Connect { get; set; }

        /// <summary>
        /// Get the query string collection of aggregated from all parameters added to the request.
        /// </summary>
        /// <returns>The <see cref="IList{T}"/> collection.</returns>
        IList<KeyValuePair<string, string>> GetQueryStringParameters();

        /// <summary>
        /// Returns the Uri for the request.
        /// </summary>
        /// <returns>The <see cref="Uri"/>.</returns>
        Uri GetUri();
    }
}