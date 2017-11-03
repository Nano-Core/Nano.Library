using System;
using System.Collections.Generic;
using Nano.Api.Extensions;
using Nano.Api.Requests.Interfaces;

namespace Nano.Api.Requests
{
    /// <summary>
    /// Get Request.
    /// </summary>
    public class GetRequest : BaseRequest
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <inheritdoc cref="IRequest.GetQueryStringParameters()"/>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                { "Id", this.Id.ToString() }
            };

            return list;
        }
    }
}