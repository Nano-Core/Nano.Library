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
        public virtual string Host { get; set; } // TODO: API: Request.Host convention

        /// <inheritdoc />
        [JsonIgnore]
        public virtual ushort Port { get; set; }// TODO: API: Request.Port convention

        /// <inheritdoc />
        [JsonIgnore]
        public virtual bool UseSsl { get; set; } = true; // TODO: API: Request.IsSsl convention

        /// <inheritdoc />
        public virtual Uri GetUri()
        {
            var scheme = this.UseSsl ? "https://" : "http://";
            var queryString = string.Join("&", this.GetQueryStringParameters().Select(x => Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value)));
            var uri = new Uri($"{scheme}{this.Host}:{this.Port}?{queryString}");

            return uri;
        }

        /// <inheritdoc />
        public abstract IList<KeyValuePair<string, string>> GetQueryStringParameters();
    }
}