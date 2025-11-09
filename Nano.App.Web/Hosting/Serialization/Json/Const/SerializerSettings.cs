using Newtonsoft.Json;

namespace Nano.Web.Hosting.Serialization.Json.Const;

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
        var serializerSettings = Models.Serialization.Json.Const.SerializerSettings.GetDefault();

        serializerSettings.ContractResolver = new MvcEntityContractResolver();

        return serializerSettings;
    }
}