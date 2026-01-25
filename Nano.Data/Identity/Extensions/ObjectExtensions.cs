using System;
using System.ComponentModel;

namespace Nano.Data.Identity.Extensions;

internal static class ObjectExtensions
{
    internal static T Parse<T>(this object @object)
    {
        ArgumentNullException.ThrowIfNull(@object);

        var convertFrom = TypeDescriptor
            .GetConverter(typeof(T))
            .ConvertFrom(@object);

        if (convertFrom == null)
        {
            return default!;
        }

        return (T)convertFrom;
    }
}