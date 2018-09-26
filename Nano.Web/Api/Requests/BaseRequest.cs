using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <inheritdoc />
    public abstract class BaseRequest : IRequest
    {
        /// <inheritdoc />
        public virtual string Action { get; set; }

        /// <inheritdoc />
        public virtual string Controller { get; set; }

        /// <inheritdoc />
        public virtual Uri GetUri(ApiOptions apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(apiOptions));

            var protocol = apiOptions.UseSsl ? "https://" : "http://";
            var parameters = this.GetQueryStringParameters()
                .Select(x => x.Value == null 
                    ? Uri.EscapeDataString(x.Key) 
                    : Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value));
            var queryString = string.Join("&", parameters);
            var uri = new Uri($"{protocol}{apiOptions.Host}:{apiOptions.Port}/{apiOptions.Root}/{this.Controller}/{this.Action}?{queryString}");

            return uri;
        }

        /// <inheritdoc />
        public virtual IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = new List<KeyValuePair<string, string>>();

            return parameters;
        }
    }
}