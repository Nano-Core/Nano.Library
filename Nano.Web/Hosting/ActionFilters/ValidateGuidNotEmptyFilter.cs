using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nano.Models;
using Nano.Models.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Nano.Web.Hosting.ActionFilters;

/// <summary>
/// Validate Guid Not Empty Filter
/// </summary>
public class ValidateGuidNotEmptyFilter : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var results = new List<string>();

        foreach (var param in context.ActionArguments)
        {
            if (param.Value == null)
            {
                continue;
            }

            if (param.Value is Guid guidValue && guidValue == Guid.Empty)
            {
                var paramInfo = context.ActionDescriptor.Parameters
                    .FirstOrDefault(p => p.Name == param.Key);

                var requiredAttribute = paramInfo?.ParameterType.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute != null)
                {
                    results
                        .Add(param.Key);
                }
            }
            else if (param.Value != null) 
            {
                if (param.Value is IEnumerable enumerableValue)
                {
                    foreach (var item in enumerableValue)
                    {
                        var nestedResults = this.ValidateGuidEmpty(item);

                        results
                            .AddRange(nestedResults);
                    }
                }
                else
                {
                    var nestedResults = this.ValidateGuidEmpty(param.Value);

                    results
                        .AddRange(nestedResults);
                }
            }
        }

        if (results.Any())
        {
            var error = new Error
            {
                Summary = "ModelState Validation Error",
                IsTranslated = true,
                Exceptions = results
                    .Select(x => $"The parameter '{x}' cannot be an empty GUID")
                    .ToArray()
            };

            context.Result = new BadRequestObjectResult(error);
        }

        base.OnActionExecuting(context);
    }

    private List<string> ValidateGuidEmpty(object obj, string path = "")
    {
        if (obj == null) 
            throw new ArgumentNullException(nameof(obj));

        var results = new List<string>();

        var properties = obj
            .GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var propertyInfo in properties)
        {
            var value = propertyInfo
                .GetValue(obj);

            if (value == null)
            {
                continue;
            }

            var currentPath = string.IsNullOrEmpty(path) 
                ? propertyInfo.Name 
                : $"{path}.{propertyInfo.Name}";

            var requiredAttribute = propertyInfo
                .GetCustomAttribute<RequiredAttribute>();

            if (propertyInfo.PropertyType == typeof(Guid) && requiredAttribute != null && (Guid)value == Guid.Empty)
            {
                results
                    .Add(currentPath);
            }
            else if (!propertyInfo.PropertyType.IsSimple())
            {
                if (value is IEnumerable enumerableValue)
                {
                    enumerableValue = (value as IDictionary)?.Values ?? enumerableValue;

                    var index = 0;
                    foreach (var item in enumerableValue)
                    {
                        var itemPath = $"{currentPath}[{index}]";

                        if (item is Guid guid && guid == Guid.Empty)
                        {
                            results
                                .Add(itemPath);
                        }
                        else if (!item.GetType().IsSimple())
                        {
                            var nestedResults = this.ValidateGuidEmpty(item, itemPath);

                            results
                                .AddRange(nestedResults);
                        }

                        index++;
                    }
                }
                else
                {
                    var nestedResults = this.ValidateGuidEmpty(value, currentPath);

                    results
                        .AddRange(nestedResults);
                }
            }
        }

        return results;
    }
}
