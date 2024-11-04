using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Nano.App.Api.Requests.Attributes;
using Nano.Models.Extensions;

namespace Nano.App.Api.Requests;

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
    public virtual string JwtTokenOverride { get; set; }

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
                var value = x.property
                    .GetValue(this);

                return value ?? string.Empty;
            });

        return parameters
            .Aggregate(string.Empty, (current, x) => current + $"{x}/");
    }

    /// <summary>
    /// Get Headers.
    /// Get the header parameters of the request, defined by properties having <see cref="RouteAttribute"/>.
    /// </summary>
    /// <returns>The route as string.</returns>
    public virtual IEnumerable<KeyValuePair<string, string>> GetHeaders()
    {
        var parameters = this
            .GetType()
            .GetProperties()
            .Select(x =>
            {
                var property = x;
                var attribute = x.GetCustomAttribute<HeaderAttribute>();

                return (property, attribute);
            })
            .Where(x => x.attribute != null)
            .Select(x =>
            {
                var name = x.property.Name;
                var value = x.property
                    .GetValue(this)?
                    .ToString();

                return new KeyValuePair<string, string>(name, $"{x.attribute.ValuePrefix}{value}");
            });

        return parameters;
    }

    /// <summary>
    /// Get Querystring.
    /// Get the querystring parameters of the request, defined by properties having <see cref="QueryAttribute"/>.
    /// </summary>
    /// <returns>The querystring as string.</returns>
    public virtual string GetQuerystring()
    {
        var querystring = this.GetQuerystringRecursive(this);

        return querystring.EndsWith('&') 
            ? querystring[..^1] 
            : querystring;
    }

    private string GetQuerystringRecursive(object value, string parentName = "")
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var str = string.Empty;

        foreach (var propertyInfo in value.GetType().GetProperties())
        {
            var attribute = propertyInfo
                .GetCustomAttribute<QueryAttribute>();

            if (attribute == null && parentName == "")
            {
                continue;
            }

            var propertyValue = propertyInfo
                .GetValue(value);

            if (propertyInfo.PropertyType.IsSimple())
            {
                str += $"{parentName}{propertyInfo.Name}={Uri.EscapeDataString(propertyValue?.ToString() ?? string.Empty)}&";
            }
            else if (propertyInfo.PropertyType.IsTypeOf(typeof(IEnumerable)))
            {
                if (propertyValue == null)
                {
                    continue;
                }

                str += ((IEnumerable)propertyValue)
                    .Cast<object>()
                    .Aggregate(string.Empty, (current, item) =>
                    {
                        if (item.GetType().IsSimple())
                        {
                            return $"{current}{parentName}{propertyInfo.Name}={Uri.EscapeDataString(item.ToString() ?? string.Empty)}&";
                        }

                        return current + this.GetQuerystringRecursive(item, $"{parentName}{propertyInfo.Name}.");
                    });
            }
            else
            {
                if (propertyValue == null)
                {
                    continue;
                }

                str += this.GetQuerystringRecursive(propertyValue, $"{parentName}{propertyInfo.Name}.");
            }
        }

        return str;
    }
}