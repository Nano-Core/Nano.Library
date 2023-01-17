using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Nano.Models.Extensions;
using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests;

/// <summary>
/// Base request (abstract).
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Jwt Token Override.
    /// Used to override the Jwt-Token for the specific request.
    /// </summary>
    [JsonIgnore]
    protected internal string JwtTokenOverride { get; set; }

    /// <summary>
    /// Controller.
    /// </summary>
    [JsonIgnore]
    protected internal string Action { get; set; }

    /// <summary>
    /// Controller.
    /// </summary>
    [JsonIgnore]
    protected internal string Controller { get; set; }

    /// <summary>
    /// Get Route.
    /// Get the route parameters of the request, defined by properties having <see cref="RouteAttribute"/>.
    /// </summary>
    /// <returns>The route as string.</returns>
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

    /// <summary>
    /// Get Querystring.
    /// Get the querystring parameters of the request, defined by properties having <see cref="QueryAttribute"/>.
    /// </summary>
    /// <returns>The querystring as string.</returns>
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
                var (property, attribute) = x;

                var key = attribute.Name ?? property.Name;
                var value = property.GetValue(this);
                var type = property.PropertyType;

                if (value == null)
                {
                    return key;
                }

                if (type.IsTypeOf(typeof(IEnumerable)) && type != typeof(string))
                {
                    var querystringPart = ((IEnumerable)value)
                        .Cast<object>()
                        .Aggregate(string.Empty, (current, item) => current + $"{key}={Uri.EscapeDataString(item?.ToString() ?? string.Empty)}&");

                    return querystringPart[..^1];
                }

                return $"{key}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}";
            });

        return string.Join("&", parameters);
    }
}