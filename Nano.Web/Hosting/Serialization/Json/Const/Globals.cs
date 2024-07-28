using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nano.Web.Hosting.Serialization.Json.Const;

/// <summary>
/// Globals.
/// </summary>
public static class Globals
{
    /// <summary>
    /// Get Json Serializer Settings.
    /// </summary>
    internal static JsonSerializerSettings GetMVcJsonSerializerSettings()
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
}