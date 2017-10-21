using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Api.Entities.Interfaces;
using Newtonsoft.Json;

namespace Nano.Api.Entities
{
    /// <inheritdoc />
    public abstract class BaseRequest : IRequest
    {
        /// <inheritdoc />
        [JsonIgnore]
        public virtual string Host { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public virtual ushort Port { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public virtual bool IsSsl { get; set; }

        /// <inheritdoc />
        public virtual Uri GetUri()
        {
            var scheme = this.IsSsl ? "https://" : "http://";
            var queryString = string.Join("&", this.GetQueryStringParameters().Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value)));
            var uri = new Uri($"{scheme}{this.Host}:{this.Port}/jsonrpc?{queryString}");

            return uri;
        }

        /// <inheritdoc />
        public abstract IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}