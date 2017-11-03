using System.Collections.Generic;
using Nano.Api.Requests.Interfaces;

namespace Nano.Api.Requests
{
    /// <summary>
    /// Query Request.
    /// </summary>
    public class QueryRequest : BaseRequest, IRequestJson
    {
        /// <inheritdoc cref="IRequest.GetQueryStringParameters()"/>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            return new List<KeyValuePair<string, string>>();
        }
    }
}