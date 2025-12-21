using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Nano.Common.Config.Helpers;

/// <summary>
/// Config Manager.
/// </summary>
public static class ConfigManager
{
    /// <summary>
    /// Path.
    /// </summary>
    public static string Path { get; }

    /// <summary>
    /// Environment.
    /// </summary>
    public static string Environment { get; }

    /// <summary>
    /// Version.
    /// </summary>
    public static Version Version { get; set; }

    /// <summary>
    /// The configuration, set when <see cref="BuildConfiguration"/> is invoked.
    /// </summary>
    internal static IConfiguration Configuration { get; set; }

    /// <summary>
    /// Constructor (static).
    /// </summary>
    static ConfigManager()
    {
        ConfigManager.Path = Directory.GetCurrentDirectory();
        ConfigManager.Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
    }

    /// <summary>
    /// Builds the <see cref="IConfiguration"/>.
    /// </summary>
    /// <returns>The <see cref="IConfiguration"/>.</returns>
    public static IConfiguration BuildConfiguration(params string[] args)
    {
        const string NAME = "appsettings";

        var path = ConfigManager.Path;
        var environment = ConfigManager.Environment;

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile($"{NAME}.json", false, true)
            .AddJsonFile($"{NAME}.{environment}.json", true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        var tempConfiguration = configurationBuilder
            .Build();

        var version = tempConfiguration
            .GetValue<string>("App:Version"); // BUG: We need to update here if we move Version

        ConfigManager.Version = new Version(version);

        if (environment == "Development")
        {
            var entryPoint = tempConfiguration
                .GetValue<string>("App:EntryPoint"); // BUG: We need to update here if we move EntryPoint

            if (entryPoint != null)
            {
                var workDir = Directory.GetCurrentDirectory();
                var entryPointFile = Directory.GetFiles(workDir, entryPoint, SearchOption.AllDirectories).FirstOrDefault();

                if (entryPointFile != null)
                {
                    configurationBuilder
                        .AddUserSecrets(Assembly.LoadFile(entryPointFile), true);
                }
            }
        }

        ConfigManager.Configuration = configurationBuilder
            .Build();

        return Configuration;
    }
}