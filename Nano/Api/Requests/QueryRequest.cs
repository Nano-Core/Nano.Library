using System.Collections.Generic;
using Nano.Api.Requests.Interfaces;

namespace Nano.Api.Requests
{
    /// <summary>
    /// Query Request.
    /// </summary>
    public class QueryRequest : BaseRequest, IRequestJson
    {
        /// <summary>
        /// Get Query String Parameters.
        /// </summary>
        /// <returns></returns>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}