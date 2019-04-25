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
        protected internal virtual string Action { get; set; }

        /// <summary>
        /// Controller.
        /// </summary>
        protected internal virtual string Controller { get; set; }

        /// <inheritdoc />
        public virtual Uri GetUri<TEntity>(ApiOptions apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(apiOptions));

            var type = typeof(TEntity);
            var action = this.Action;
            var protocol = apiOptions.UseSsl ? "https://" : "http://";
            var controller = this.Controller ?? (type.IsGenericType
                ? $"{type.GenericTypeArguments[0].Name}s"
                : $"{type.Name.ToLower()}s");
            var routeParameters = this.GetRouteParameters()
                .Aggregate("", (x, y) => x + $"/{y}");
            var queryParameters = this.GetQueryStringParameters()
                .Select(x => x.Value == null 
                    ? Uri.EscapeDataString(x.Key) 
                    : Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value));
            var queryString = string.Join("&", queryParameters);

            return new Uri($"{protocol}{apiOptions.Host}:{apiOptions.Port}/{apiOptions.Root}/{controller}/{action}{routeParameters}?{queryString}");
        }

        /// <inheritdoc />
        public virtual IList<string> GetRouteParameters()
        {
            var parameters = new List<string>();

            return parameters;
        }

        /// <inheritdoc />
        public virtual IList<KeyValuePair<string, string>> GetQueryStringParameters()
        {
            var parameters = new List<KeyValuePair<string, string>>();

            return parameters;
        }
    }
}