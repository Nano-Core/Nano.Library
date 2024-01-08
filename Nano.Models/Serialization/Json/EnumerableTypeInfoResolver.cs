using System;
using System.Collections;
using System.Text.Json.Serialization.Metadata;

namespace Nano.Models.Serialization.Json
{
    /// <summary>
    /// Enumerable Type Info Resolver.
    /// </summary>
    public static class EnumerableTypeInfoResolver
    {
        /// <summary>
        /// Ignore Empty Collections.
        /// </summary>
        /// <param name="typeInfo"></param>
        public static void IgnoreEmptyCollections(JsonTypeInfo typeInfo)
        {
            if (typeInfo == null)
                throw new ArgumentNullException(nameof(typeInfo));

            foreach (var property in typeInfo.Properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    // ReSharper disable NotDisposedResource
                    property.ShouldSerialize = (_, value) => value is IEnumerable collection && collection.GetEnumerator().MoveNext();
                    // ReSharper restore NotDisposedResource
                }
            }
        }
    }
}
