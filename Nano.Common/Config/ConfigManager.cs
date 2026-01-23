using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Nano.Common.Config;

/// <summary>
/// Config Manager.
/// </summary>
public static class ConfigManager
{
    internal static IConfiguration Configuration { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="environment"></param>
    /// <param name="entryAssembly"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IConfiguration BuildConfiguration(string environment, Assembly? entryAssembly = null, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var path = Directory.GetCurrentDirectory();
        var stream = ConfigManager.LoadConfigurationStream(path, environment);

        if (entryAssembly == null)
        {
            throw new NullReferenceException(nameof(entryAssembly));
        }

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonStream(stream)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddUserSecrets(entryAssembly, true, true);

        return ConfigManager.Configuration = configurationBuilder
            .Build();
    }


    private static Stream LoadConfigurationStream(string path, string environment)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(environment);

        const string APP_SETTINGS = "appsettings";

        var baseJsonPath = Path.Combine(path, $"{APP_SETTINGS}.json");
        var envJsonPath = Path.Combine(path, $"{APP_SETTINGS}.{environment}.json");

        var baseJson = File.ReadAllText(baseJsonPath);
        var baseObj = JObject.Parse(baseJson);

        if (File.Exists(envJsonPath))
        {
            var envJson = File.ReadAllText(envJsonPath);
            var envObj = JObject.Parse(envJson);

            RemoveNulls(baseObj, envObj);
        }

        var mergedJson = baseObj.ToString(Newtonsoft.Json.Formatting.None);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(mergedJson));

        return stream;
    }
    private static void RemoveNulls(JObject baseJson, JObject overrideJson)
    {
        foreach (var property in overrideJson.Properties())
        {
            var overrideValue = property.Value;

            // Explicit `"key": null` → remove from base
            if (overrideValue.Type == JTokenType.Null)
            {
                baseJson.Remove(property.Name);
                continue;
            }

            // Nested object → recurse
            if (overrideValue is JObject overrideObj &&
                baseJson[property.Name] is JObject baseObj)
            {
                RemoveNulls(baseObj, overrideObj);
                continue;
            }

            // Any other value (arrays, primitives, objects)
            // → replace using deep clone
            baseJson[property.Name] = overrideValue.DeepClone();
        }
    }
}
