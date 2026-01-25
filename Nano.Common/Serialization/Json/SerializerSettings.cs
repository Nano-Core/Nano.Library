using System.Globalization;
using Nano.Common.Serialization.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nano.Common.Serialization.Json;

/// <summary>
/// Provides default JSON serializer settings for the application.
/// </summary>
public static class SerializerSettings
{
    /// <summary>
    /// Gets the default <see cref="JsonSerializerSettings"/> configured with:
    /// <list type="bullet">
    ///     <item>MaxDepth = 128</item>
    ///     <item>Current culture for formatting</item>
    ///     <item>Null values ignored</item>
    ///     <item>Reference loop handling ignored</item>
    ///     <item>No reference preservation</item>
    ///     <item>Uses <see cref="DefaultEntityContractResolver"/></item>
    ///     <item>Includes converters for string enums and geometries (case-insensitive)</item>
    /// </list>
    /// </summary>
    /// <returns>A configured <see cref="JsonSerializerSettings"/> instance.</returns>
    public static JsonSerializerSettings GetDefault()
    {
        return new JsonSerializerSettings
        {
            MaxDepth = 128,
            Culture = CultureInfo.CurrentCulture,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new DefaultEntityContractResolver(),
            Converters =
            [
                new StringEnumConverter(),
                new GeometryConverterIgnoreCase()
            ]
        };
    }
}