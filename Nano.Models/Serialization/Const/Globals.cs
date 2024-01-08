using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Nano.Models.Serialization.Json;

namespace Nano.Models.Serialization.Const;

/// <summary>
/// Globals.
/// </summary>
public static class Globals
{
    /// <summary>
    /// Json Serializer Settings.
    /// </summary>
    public static readonly JsonSerializerOptions jsonSerializerSettings = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        MaxDepth = 128,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                LazyLoaderTypeInfoResolver.IgnoreLazyLoader,
                EnumerableTypeInfoResolver.IgnoreEmptyCollections
            }
        },
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}