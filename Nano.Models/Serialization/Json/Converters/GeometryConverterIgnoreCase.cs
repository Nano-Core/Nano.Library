using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nano.Models.Serialization.Json.Converters;

/// <summary>
/// Geometry Converter Ignore Case.
/// </summary>
public class GeometryConverterIgnoreCase : GeometryConverter
{
    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader == null) 
            throw new ArgumentNullException(nameof(reader));
        
        if (objectType == null) 
            throw new ArgumentNullException(nameof(objectType));
        
        if (existingValue == null) 
            throw new ArgumentNullException(nameof(existingValue));
        
        if (serializer == null) 
            throw new ArgumentNullException(nameof(serializer));
        
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Object)
        {
            var obj = (JObject)token;

            if (objectType.IsSubclassOf(typeof(Geometry)))
            {
                this.RenameProperty(obj, "Type", "type");
                this.RenameProperty(obj, "Coordinates", "coordinates");
                this.RenameProperty(obj, "Geometries", "geometries");
            }

            var jsonReader = new JTokenReader(obj);

            jsonReader
                .Read();

            return base.ReadJson(jsonReader, objectType, existingValue, serializer);
        }

        throw new JsonSerializationException("Expected object");
    }

    private void RenameProperty(JObject @object, string from, string to)
    {
        if (@object == null) 
            throw new ArgumentNullException(nameof(@object));
        
        if (from == null) 
            throw new ArgumentNullException(nameof(from));
        
        if (to == null) 
            throw new ArgumentNullException(nameof(to));
        
        if (@object.TryGetValue(from, StringComparison.OrdinalIgnoreCase, out var value))
        {
            @object
                .Remove(from);
            
            @object[to] = value;
        }
    }
}