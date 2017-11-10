using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Api.Requests.Interfaces;
using Newtonsoft.Json;

namespace Nano.Api.Requests
{
    /// <inheritdoc />
    public abstract class BaseRequest : IRequest
    {
        /// <inheritdoc />
        [JsonIgnore]
        public virtual ApiConnect Connect { get; set; }

        /// <inheritdoc />
        public virtual Uri GetUri()
        {
            var queryString = string.Join("&", this.GetQueryStringParameters().Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value)));
            var uri = new Uri($"{this.Connect.AbsoluteUrl}?{queryString}");

            return uri;
        }

        /// <inheritdoc />
        public abstract IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}