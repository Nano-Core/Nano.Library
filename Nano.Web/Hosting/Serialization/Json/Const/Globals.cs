using Newtonsoft.Json;

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
        var serializerSettings = Models.Serialization.Json.Const.Globals.GetDefaultJsonSerializerSettings();

        serializerSettings.ContractResolver = new MvcEntityContractResolver();

        return serializerSettings;
    }
}