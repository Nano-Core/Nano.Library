using System.Globalization;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nano.Models.Serialization.Json.Const;

/// <summary>
/// Globals.
/// </summary>
public static class Globals
{
    /// <summary>
    /// Get Json Serializer Settings.
    /// </summary>
    public static JsonSerializerSettings GetDefaultJsonSerializerSettings()
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
                new GeometryConverter()
            ]
        };
    }
}