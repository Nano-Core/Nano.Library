using System;
using System.ComponentModel;

namespace Nano.Models.Extensions;

/// <summary>
/// Object Extensions.
/// </summary>
public static class ObjectExtensions
{
    internal static T Parse<T>(this object @object)
    {
        if (@object == null)
            throw new ArgumentNullException(nameof(@object));

        return (T)TypeDescriptor
            .GetConverter(typeof(T))
            .ConvertFrom(@object.ToString());
    }
}