using Newtonsoft.Json;

namespace Nano.App.Web.Mvc.Serialization.Json;

/// <summary>
/// Serializer Settings.
/// </summary>
public static class SerializerSettings
{
    /// <summary>
    /// Get Json Serializer Settings.
    /// </summary>
    internal static JsonSerializerSettings GetMVcJsonSerializerSettings()
    {
        var serializerSettings = Common.Serialization.Json.SerializerSettings.GetDefault();

        serializerSettings.ContractResolver = new MvcEntityContractResolver();

        return serializerSettings;
    }
}