using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nano.Data.Eventing.Models;

internal sealed class PropertyAccessors : Dictionary<(Type Type, string Path), Func<object, object?>>
{
    internal Func<object, object?> CreateAccessor(Type rootType, string path)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(path);

        var parameter = Expression.Parameter(typeof(object), "obj");
        Expression current = Expression.Convert(parameter, rootType);

        foreach (var part in path.Split('.'))
        {
            var property = current.Type
                .GetProperty(part) ?? throw new InvalidOperationException($"Property '{part}' not found on '{current.Type.Name}'");

            var nullCheck = Expression.Equal(current, Expression.Constant(null));
            var defaultValue = Expression.Default(property.PropertyType);
            var accessor = Expression.Property(current, property);

            current = Expression.Condition(nullCheck, defaultValue, accessor);
        }

        var lambda = Expression.Lambda<Func<object, object?>>(Expression.Convert(current, typeof(object)), parameter);

        return lambda
            .Compile();
    }
}