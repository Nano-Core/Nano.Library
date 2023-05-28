using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Nano.Web.Hosting.Serialization.Json;

namespace Nano.Web.Const;

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