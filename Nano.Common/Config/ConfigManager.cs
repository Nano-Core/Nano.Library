using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Nano.Common.Config;

/// <summary>
/// Provides centralized access to the application <see cref="IConfiguration"/> and handles building it
/// from JSON files, environment variables, command-line arguments, and user secrets.
/// </summary>
public static class ConfigManager
{
    internal static IConfiguration Configuration { get; set; } = null!;

    /// <summary>
    /// Builds the application <see cref="IConfiguration"/> by loading configuration
    /// from JSON files, environment variables, command-line arguments, and user secrets.
    /// </summary>
    /// <param name="environment">The environment name (e.g., "Development", "Production") used to load environment-specific configuration.</param>
    /// <param name="entryAssembly">The entry <see cref="Assembly"/> used for loading user secrets. Must not be null.</param>
    /// <param name="args">Optional command-line arguments to include in the configuration.</param>
    /// <returns>The fully built <see cref="IConfiguration"/> instance. Also sets <see cref="ConfigManager.Configuration"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="environment"/> is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if <paramref name="entryAssembly"/> is null.</exception>
    public static IConfiguration BuildConfiguration(string environment, Assembly? entryAssembly = null, params string[] args)
    {
        ArgumentNullException.ThrowIfNull(environment);

        var path = Directory.GetCurrentDirectory();

        using var stream = LoadConfigurationStream(path, environment);

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonStream(stream)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        if (entryAssembly != null)
        {
            configurationBuilder
                .AddUserSecrets(entryAssembly, true, true);
        }

        return Configuration = configurationBuilder
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

        EnsureNonEmptyObjects(baseObj);

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
    private static void EnsureNonEmptyObjects(JObject jObject)
    {
        ArgumentNullException.ThrowIfNull(jObject);

        foreach (var property in jObject.Properties())
        {
            if (property.Value is JObject child)
            {
                if (!child.HasValues)
                {
                    child["__placeholder"] = true;

                    continue;
                }

                EnsureNonEmptyObjects(child);
            }
        }
    }
    private static void RemoveNulls(JObject baseJson, JObject overrideJson)
    {
        ArgumentNullException.ThrowIfNull(baseJson);
        ArgumentNullException.ThrowIfNull(overrideJson);

        foreach (var property in overrideJson.Properties())
        {
            var overrideValue = property.Value;

            if (overrideValue.Type == JTokenType.Null)
            {
                baseJson
                    .Remove(property.Name);

                continue;
            }

            if (overrideValue is JObject overrideObj)
            {
                if (!overrideObj.HasValues)
                {
                    baseJson[property.Name] = new JObject
                    {
                        ["__empty"] = true
                    };

                    continue;
                }

                if (baseJson[property.Name] is JObject baseObj)
                {
                    RemoveNulls(baseObj, overrideObj);

                    continue;
                }
            }

            baseJson[property.Name] = overrideValue.DeepClone();
        }
    }
}
