using System.Globalization;
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
    public static JsonSerializerSettings GetMVcJsonSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings
        {
            MaxDepth = 128,
            Culture = CultureInfo.CurrentCulture,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new MvcEntityContractResolver()
        };

        serializerSettings.Converters
            .Add(new StringEnumConverter());

        return serializerSettings;
    }

    /// <summary>
    /// Get Json Serializer Settings.
    /// </summary>
    public static JsonSerializerSettings GetDefaultJsonSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings
        {
            MaxDepth = 128,
            Culture = CultureInfo.CurrentCulture,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ContractResolver = new DefaultEntityContractResolver()
        };

        serializerSettings.Converters
            .Add(new StringEnumConverter());

        return serializerSettings;
    }
}