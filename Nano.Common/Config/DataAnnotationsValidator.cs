using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Nano.Common.Config;

internal class DataAnnotationsValidator
{
    internal static bool TryValidateObjectRecursive<T>(T obj, List<ValidationResult> results, IDictionary<object, object>? validationContextItems = null)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(results);

        return TryValidateObjectRecursive(obj, results, new HashSet<object>(), validationContextItems);
    }


    private static bool TryValidateObjectRecursive<T>(T obj, ICollection<ValidationResult> results, ISet<object> validatedObjects, IDictionary<object, object>? validationContextItems)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(results);

        if (!validatedObjects.Add(obj))
        {
            return true;
        }

        var result = TryValidateObject(obj, results, validationContextItems);

        var properties = obj
            .GetType()
            .GetProperties()
            .Where(x => x.CanRead && x.GetIndexParameters().Length == 0)
            .ToArray();

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
            {
                continue;
            }

            var value = obj
                .GetType()
                .GetProperty(property.Name)?
                .GetValue(obj, null) ?? string.Empty;

            if (value is IEnumerable asEnumerable)
            {
                foreach (var enumObj in asEnumerable)
                {
                    if (enumObj == null)
                    {
                        continue;
                    }

                    var nestedResults = new List<ValidationResult>();

                    if (TryValidateObjectRecursive(enumObj, nestedResults, validatedObjects, validationContextItems))
                    {
                        continue;
                    }

                    result = false;

                    foreach (var validationResult in nestedResults)
                    {
                        var property1 = property;

                        results
                            .Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                    }
                }
            }
            else
            {
                var nestedResults = new List<ValidationResult>();

                if (TryValidateObjectRecursive(value, nestedResults, validatedObjects, validationContextItems))
                {
                    continue;
                }

                result = false;

                foreach (var validationResult in nestedResults)
                {
                    var property1 = property;

                    results
                        .Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property1.Name + '.' + x)));
                }
            }
        }

        return result;
    }
    private static bool TryValidateObject(object obj, ICollection<ValidationResult> results, IDictionary<object, object>? validationContextItems)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(results);

        return Validator.TryValidateObject(obj, new ValidationContext(obj, null, validationContextItems!), results, true);
    }
}