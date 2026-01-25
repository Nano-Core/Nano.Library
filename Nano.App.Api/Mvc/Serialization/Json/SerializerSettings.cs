using Newtonsoft.Json;

namespace Nano.App.Api.Mvc.Serialization.Json;

internal static class SerializerSettings
{
    internal static JsonSerializerSettings GetMVcJsonSerializerSettings()
    {
        var serializerSettings = Common.Serialization.Json.SerializerSettings.GetDefault();

        serializerSettings.ContractResolver = new MvcEntityContractResolver();

        return serializerSettings;
    }
}