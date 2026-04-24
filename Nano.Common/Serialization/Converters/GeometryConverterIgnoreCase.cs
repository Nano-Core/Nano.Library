using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nano.Common.Serialization.Converters;

/// <summary>
/// A <see cref="GeometryConverter"/> that ignores case when reading JSON properties for geometries.
/// </summary>
public class GeometryConverterIgnoreCase : GeometryConverter
{
    /// <summary>
    /// Reads JSON into a <see cref="Geometry"/> object, renaming properties to match expected casing.
    /// </summary>
    /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
    /// <param name="objectType">The type of object to create.</param>
    /// <param name="existingValue">The existing value of the object being read.</param>
    /// <param name="serializer">The <see cref="JsonSerializer"/> used for deserialization.</param>
    /// <returns>The deserialized geometry object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="reader"/>, <paramref name="objectType"/>, or <paramref name="serializer"/> is null.</exception>
    /// <exception cref="JsonSerializationException">Thrown if the JSON token is not an object.</exception>
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(objectType);
        ArgumentNullException.ThrowIfNull(serializer);

        var token = JToken.Load(reader);

        if (token.Type != JTokenType.Object)
        {
            throw new JsonSerializationException("Expected object");
        }

        var obj = (JObject)token;

        if (objectType.IsSubclassOf(typeof(Geometry)))
        {
            RenameProperty(obj, "Type", "type");
            RenameProperty(obj, "Coordinates", "coordinates");
            RenameProperty(obj, "Geometries", "geometries");
        }

        using var jsonReader = new JTokenReader(obj);

        jsonReader
            .Read();

        return base.ReadJson(jsonReader, objectType, existingValue, serializer);
    }


    private static void RenameProperty(JObject @object, string from, string to)
    {
        ArgumentNullException.ThrowIfNull(@object);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);

        if (!@object.TryGetValue(from, StringComparison.OrdinalIgnoreCase, out var value))
        {
            return;
        }

        @object
            .Remove(from);

        @object[to] = value;
    }
}