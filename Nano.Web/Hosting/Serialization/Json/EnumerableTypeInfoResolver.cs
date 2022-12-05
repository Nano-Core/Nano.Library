using System.Collections;
using System.Text.Json.Serialization.Metadata;

namespace Nano.Web.Hosting.Serialization.Json
{
    /// <summary>
    /// Enumerable Type Info Resolver.
    /// </summary>
    internal static class EnumerableTypeInfoResolver
    {
        /// <summary>
        /// Ignore Empty Collections.
        /// </summary>
        /// <param name="typeInfo"></param>
        internal static void IgnoreEmptyCollections(JsonTypeInfo typeInfo)
        {
            foreach (var property in typeInfo.Properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    continue;
                }

                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    property.ShouldSerialize = (_, value) => value is IEnumerable collection && collection.GetEnumerator().MoveNext();
                }
            }
        }
    }
}
