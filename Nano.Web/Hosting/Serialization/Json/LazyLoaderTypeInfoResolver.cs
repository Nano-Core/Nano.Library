using System;
using System.Text.Json.Serialization.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Nano.Web.Hosting.Serialization.Json;

/// <summary>
/// Lazy Loader Type Info Resolver.
/// </summary>
internal static class LazyLoaderTypeInfoResolver
{
    /// <summary>
    /// Ignore Empty Collections.
    /// </summary>
    /// <param name="typeInfo"></param>
    internal static void IgnoreLazyLoader(JsonTypeInfo typeInfo)
    {
        if (typeInfo == null)
            throw new ArgumentNullException(nameof(typeInfo));

        foreach (var property in typeInfo.Properties)
        {
            property.ShouldSerialize = property.PropertyType == typeof(ILazyLoader)
                ? (_, _) => false
                : (_, _) => true;
        }
    }
}