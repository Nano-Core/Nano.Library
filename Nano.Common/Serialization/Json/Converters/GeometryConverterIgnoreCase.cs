using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nano.Common.Serialization.Json.Converters;

/// <summary>
/// Geometry Converter Ignore Case.
/// </summary>
public class GeometryConverterIgnoreCase : GeometryConverter
{
    /// <inheritdoc />
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

        var jsonReader = new JTokenReader(obj);

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