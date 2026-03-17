using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Nano.Data.Extensions;

internal static class DbDataReaderExtensions
{
    internal static T Map<T>(this DbDataReader reader)
        where T : new()
    {
        ArgumentNullException.ThrowIfNull(reader);

        var obj = new T();
        var map = MapperCache<T>.Map;

        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (!map.TryGetValue(reader.GetName(i), out var property))
            {
                continue;
            }

            if (reader.IsDBNull(i))
            {
                continue;
            }

            property
                .SetValue(obj, reader.GetValue(i));
        }

        return obj;
    }


    private static class MapperCache<T>
    {
        public static readonly Dictionary<string, PropertyInfo> Map = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
    }
}