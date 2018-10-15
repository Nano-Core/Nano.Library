using System;
using System.Collections.Generic;
using System.Linq;
using Nano.Web.Api.Requests.Interfaces;

namespace Nano.Web.Api.Requests
{
    /// <inheritdoc />
    public abstract class BaseRequest : IRequest
    {
        /// <summary>
        /// Controller.
        /// </summary>
        public virtual string Action { get; protected set; }

        /// <summary>
        /// Controller.
        /// </summary>
        public virtual string Controller { get; protected set; }

        /// <inheritdoc />
        public virtual Uri GetUri<TResponse>(ApiOptions apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(apiOptions));

            var type = typeof(TResponse);
            var protocol = apiOptions.UseSsl ? "https://" : "http://";
            var controller = this.Controller ?? (type.IsGenericType
                ? $"{type.GenericTypeArguments[0].Name}s"
                : $"{type.Name.ToLower()}s");
            var parameters = this.GetQueryStringParameters()
                .Select(x => x.Value == null 
                    ? Uri.EscapeDataString(x.Key) 
                    : Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value));
            var queryString = string.Join("&", parameters);
            var uri = new Uri($"{protocol}{apiOptions.Host}:{apiOptions.Port}/{apiOptions.Root}/{controller}/{this.Action}?{queryString}");

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