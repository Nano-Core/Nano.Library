using System;
using System.Collections.Generic;

namespace Nano.Api.Entities.Interfaces
{
    /// <summary>
    /// Base interface for requests.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Host
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Port.
        /// </summary>
        ushort Port { get; set; }

        /// <summary>
        /// Is Ssl.
        /// Determines if http or https is used when submitting the request.
        /// </summary>
        bool IsSsl { get; set; }

        /// <summary>
        /// Returns the Uri for the request.
        /// </summary>
        /// <returns>The <see cref="Uri"/>.</returns>
        Uri GetUri();

        /// <summary>
        /// Get the query string collection of aggregated from all parameters added to the request.
        /// </summary>
        /// <returns>The <see cref="IList{KeyValuePair}"/> collection.</returns>
        IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}