using System;
using System.Linq;
using System.Reflection;
using Nano.Web.Api.Requests.Attributes;
using Nano.Web.Api.Requests.Interfaces;
using Newtonsoft.Json;

namespace Nano.Web.Api.Requests
{
    /// <inheritdoc />
    public abstract class BaseRequest : IRequest
    {
        /// <summary>
        /// Controller.
        /// </summary>
        [JsonIgnore]
        public string Action { get; set; }

        /// <summary>
        /// Controller.
        /// </summary>
        [JsonIgnore]
        public string Controller { get; set; }

        /// <inheritdoc />
        public virtual string GetRoute()
        {
            var parameters = this
                .GetType()
                .GetProperties()
                .Select(x =>
                {
                    var property = x;
                    var attribute = x.GetCustomAttribute<RouteAttribute>();

                    return (property, attribute);
                })
                .Where(x => x.attribute != null)
                .OrderBy(x => x.attribute.Order)
                .Select(x =>
                {
                    var value = x.property.GetValue(this);

                    return value ?? string.Empty;
                });

            return parameters
                .Aggregate(string.Empty, (current, x) => current + $"{x}/");
        }

        /// <inheritdoc />
        public virtual string GetQuerystring()
        {
            var parameters = this
                .GetType()
                .GetProperties()
                .Select(x =>
                {
                    var property = x;
                    var attribute = x.GetCustomAttribute<QueryAttribute>();

                    return (property, attribute);
                })
                .Where(x => x.attribute != null)
                .Select(x =>
                {
                    var key = x.attribute.Name ?? x.property.Name;
                    var value = x.property.GetValue(this);

                    if (value == null)
                        return Uri.EscapeDataString(key);

                    return Uri.EscapeDataString(key) + "=" + Uri.EscapeDataString(value.ToString() ?? string.Empty);
                });

            return string.Join("&", parameters);
        }
    }
}