using Nano.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.ApiClient.Models;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents the base request class for all HTTP requests.
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Optional JWT token to override the default authentication token for this request.
    /// </summary>
    [JsonIgnore]
    public virtual string? JwtTokenOverride { get; set; }

    /// <summary>
    /// Optional controller name for the request.
    /// </summary>
    [JsonIgnore]
    protected internal string? Controller { get; set; }

    internal HttpMethod GetMethod()
    {
        var httpMethod = this
            .GetType()
            .GetCustomAttribute<ActionAttribute>()?
            .Method;

        return httpMethod ?? throw new NullReferenceException(nameof(httpMethod));
    }

    internal virtual IEnumerable<KeyValuePair<string, string>> GetHeaders()
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
                var name = x.attribute!.Name ?? x.property.Name;

                var value = x.property
                    .GetValue(this)?
                    .ToString();

                return new KeyValuePair<string, string>(name, $"{x.attribute!.ValuePrefix}{value}");
            });

        return parameters;
    }

    internal virtual string GetAction()
    {
        var action = this
            .GetType()
            .GetCustomAttribute<ActionAttribute>()?
            .ActionTemplate;

        return action ?? throw new NullReferenceException(nameof(action));
    }

    internal virtual IEnumerable<object> GetRouteParameters()
    {
        return this
            .GetType()
            .GetProperties()
            .Select(x =>
            {
                var property = x;
                var attribute = x.GetCustomAttribute<RouteAttribute>();

                return (property, attribute);
            })
            .Where(x => x.attribute != null)
            .OrderBy(x => x.attribute!.Order)
            .Select(x =>
            {
                var value = x.property
                    .GetValue(this);

                return value ?? string.Empty;
            });
    }

    internal virtual string GetQuerystring()
    {
        var querystring = this.GetQuerystringRecursive(this);

        return querystring.EndsWith('&')
            ? querystring[..^1]
            : querystring;
    }

    internal virtual object? GetBody()
    {
        return this
            .GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<BodyAttribute>() != null)
            .Select(x => x.GetValue(this))
            .FirstOrDefault();
    }

    internal virtual IEnumerable<FormItem> GetForm()
    {
        return this
            .GetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<FormAttribute>() != null)
            .Select(x =>
            {
                var value = x.GetValue(this);

                return new FormItem
                {
                    Name = x.Name,
                    Value = value
                };
            });
    }


    private string GetQuerystringRecursive(object value, string parentName = "")
    {
        ArgumentNullException.ThrowIfNull(value);

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

            var name = attribute?.Name ?? propertyInfo.Name;

            if (propertyInfo.PropertyType.IsSimple())
            {
                str += $"{parentName}{name}={Uri.EscapeDataString(propertyValue?.ToString() ?? string.Empty)}&";
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
                            return $"{current}{parentName}{name}={Uri.EscapeDataString(item.ToString() ?? string.Empty)}&";
                        }

                        return current + this.GetQuerystringRecursive(item, $"{parentName}{name}.");
                    });
            }
            else
            {
                if (propertyValue == null)
                {
                    continue;
                }

                str += this.GetQuerystringRecursive(propertyValue, $"{parentName}{name}.");
            }
        }

        return str;
    }
}