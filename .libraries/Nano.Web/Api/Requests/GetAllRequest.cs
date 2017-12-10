using System.Collections.Generic;

namespace Nano.Web.Api.Requests
{
    /// <summary>
    /// Get All Request.
    /// </summary>
    public class GetAllRequest : BaseRequest
    {
        /// <inheritdoc />
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var list = new List<KeyValuePair<string, string>>();

            return list;
        }
    }
}