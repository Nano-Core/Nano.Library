using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nano.Models.Extensions;

/// <summary>
/// Type Extensions.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets a friendly display name for the <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> from which to get all parent types.</param>
    /// <returns>A friendly display name.</returns>
    public static string GetFriendlyName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var friendlyName = type.FullName;

        if (type.IsGenericType)
        {
            friendlyName = type.GetTypeString();
        }

        return friendlyName;
    }

    /// <summary>
    /// Gets whehter the passed <paramref name="type"/> derives or implements the passed <paramref name="baseType"/>.
    /// All base classes and interfaces are inspected.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> from which to determine, if the passed <paramref name="baseType"/> is parent.</param>
    /// <param name="baseType">The <see cref="Type"/> that be inherited or implemented by the passed <paramref name="type"/>.</param>
    /// <returns>True or false, depending on whehter the <paramref name="type"/> implements or derives from <paramref name="baseType"/>.</returns>
    public static bool IsTypeOf(this Type type, Type baseType)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (baseType == null)
            throw new ArgumentNullException(nameof(baseType));

        return type
            .GetParentTypes()
            .Any(x =>
                x == baseType ||
                x.IsGenericType && x.GetGenericTypeDefinition() == baseType);
    }

    /// <summary>
    /// Is Simple.
    /// Checks if the <see cref="Type"/> is simple.
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <returns>Boolean indicating if the type is simple.</returns>
    public static bool IsSimple(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(Guid)
               || type == typeof(TimeSpan)
               || type == typeof(TimeOnly)
               || type == typeof(DateOnly)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(Nullable<>);
    }

    private static string GetTypeString(this Type type)
    {
        var output = new StringBuilder();
        var qualifiedName = type.AssemblyQualifiedName;
        var backTick = qualifiedName?.IndexOf('`') + 1 ?? 0;

        output
            .Append(qualifiedName?[..(backTick - 1)].Replace("[", string.Empty));

        var typeStrings = type
            .GetGenericArguments()
            .Select(x => x.IsGenericType
                ? x.GetTypeString()
                : x.ToString())
            .ToList();

        output.Append($"<{string.Join(",", typeStrings)}>");
        return output.ToString();
    }
    private static IEnumerable<Type> GetBaseTypes(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var types = new List<Type>();

        while (type.BaseType != null)
        {
            types.Add(type);

            type = type.BaseType;
        }

        return types;
    }
    private static IEnumerable<Type> GetParentTypes(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        return type
            .GetInterfaces()
            .Concat(type.GetBaseTypes());
    }
}