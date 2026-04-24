using System;

namespace Nano.App.Api.Extensions;

internal static class TypeExtensions
{
    internal static Type? GetGenericBaseType(this Type type, Type genericBaseType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericBaseType);

        while (type != typeof(object))
        {
            var current = type.IsGenericType
                ? type.GetGenericTypeDefinition()
                : type;

            if (current == genericBaseType)
                return type;

            type = type.BaseType ?? throw new NullReferenceException(nameof(type.BaseType));
        }

        return null;
    }
}