using Newtonsoft.Json;

namespace Nano.App.Web.Hosting.Serialization.Json.Const;

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
        var serializerSettings = Common.Serialization.SerializerSettings.GetDefault();

        serializerSettings.ContractResolver = new MvcEntityContractResolver();

        return serializerSettings;
    }
}