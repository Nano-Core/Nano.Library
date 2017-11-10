using System.Collections.Generic;
using Nano.Api.Requests.Interfaces;

namespace Nano.Api.Requests
{
    /// <summary>
    /// Get All Request.
    /// </summary>
    public class GetAllRequest : BaseRequest
    {
        /// <inheritdoc cref="IRequest.GetQueryStringParameters()"/>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var list = new List<KeyValuePair<string, string>>();

            return list;
        }
    }
}