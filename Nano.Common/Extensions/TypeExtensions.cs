using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nano.Common.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Type"/> class to inspect type hierarchy,
/// determine simplicity, and generate friendly type names.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified <paramref name="type"/> derives from or implements the specified <paramref name="baseType"/>.
    /// All base classes and interfaces are inspected, including generic definitions.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <param name="baseType">The <see cref="Type"/> that may be a base class or interface.</param>
    /// <returns><c>true</c> if <paramref name="type"/> implements or derives from <paramref name="baseType"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or <paramref name="baseType"/> is null.</exception>
    public static bool IsTypeOf(this Type type, Type baseType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(baseType);

        var a = type
            .GetParentTypes()
            .ToArray();

        return a
            .Any(x =>
                x == baseType ||
                (x.IsGenericType && x.GetGenericTypeDefinition() == baseType));
    }

    /// <summary>
    /// Determines whether the <see cref="Type"/> is a simple type.
    /// Simple types include primitives, enums, strings, <see cref="Guid"/>, <see cref="TimeSpan"/>, date/time types, and nullable versions.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns><c>true</c> if the type is simple; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    public static bool IsSimple(this Type type)
    {
        while (true)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);

                type = underlyingType ?? throw new NullReferenceException(nameof(underlyingType));

                continue;
            }

            return
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(Guid) ||
                type == typeof(TimeSpan) ||
                type == typeof(TimeOnly) ||
                type == typeof(DateOnly) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset);
        }
    }

    /// <summary>
    /// Gets a friendly display name for the <see cref="Type"/>, including generic type arguments.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to generate a friendly name.</param>
    /// <returns>A readable type name with generic parameters if applicable.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if the friendly name could not be generated.</exception>
    public static string GetFriendlyName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var friendlyName = type.FullName;

        if (type.IsGenericType)
        {
            friendlyName = type
                .GetTypeString();
        }

        return friendlyName ?? throw new NullReferenceException(nameof(friendlyName));
    }


    private static IEnumerable<Type> GetParentTypes(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type
            .GetInterfaces()
            .Concat(type.GetBaseTypes());
    }
    private static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var current = type.BaseType;

        while (current != null && current != typeof(object))
        {
            yield return current;

            current = current.BaseType;
        }
    }
    private static string GetTypeString(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var output = new StringBuilder();
        var qualifiedName = type.AssemblyQualifiedName;
        var backTick = qualifiedName?.IndexOf('`') + 1 ?? 0;

        output
            .Append(qualifiedName?[..(backTick - 1)]?.Replace("[", string.Empty));

        var typeStrings = type
            .GetGenericArguments()
            .Select(x => x.IsGenericType
                ? x.GetTypeString()
                : x.ToString())
            .ToList();

        output
            .Append($"<{string.Join(",", typeStrings)}>");

        return output
            .ToString();
    }
}