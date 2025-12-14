using System;
using System.Linq;
using System.Text;

namespace Nano.Eventing.RabbitMq.Extensions;

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
            friendlyName = type
                .GetTypeString();
        }

        return friendlyName;
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
}