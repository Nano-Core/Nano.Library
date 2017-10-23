using System;
using System.Collections.Generic;
using Nano.Common.Extensions;

namespace Nano.Api.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class GetRequest : BaseRequest
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var list = new List<KeyValuePair<string, string>>
            {
                {"id", this.Id.ToString()}
            };

            return list;
        }
    }
}