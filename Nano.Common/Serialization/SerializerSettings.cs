using System.Globalization;
using Nano.Common.Serialization.Json;
using Nano.Common.Serialization.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nano.Common.Serialization;

/// <summary>
/// Json Serializer Settings.
/// </summary>
public static class SerializerSettings
{
    /// <summary>
    /// Get Json Serializer Settings.
    /// </summary>
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