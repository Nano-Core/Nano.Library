using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Nano.Common.Config;

/// <summary>
/// Config Manager.
/// </summary>
public static class ConfigManager
{
    internal static IConfiguration Configuration { get; set; } = null!;

    /// <summary>
    /// Builds the <see cref="IConfiguration"/>.
    /// </summary>
    /// <returns>The <see cref="IConfiguration"/>.</returns>
    public static IConfiguration BuildConfiguration(params string[] args)
    {
        const string NAME = "appsettings";

        var path = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile($"{NAME}.json", false, true)
            .AddJsonFile($"{NAME}.{environment}.json", true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        var tempConfiguration = configurationBuilder
            .Build();

        if (environment == "Development")
        {
            var entryPoint = tempConfiguration
                .GetValue<string>("App:EntryPoint");

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

        return ConfigManager.Configuration = configurationBuilder
            .Build();
    }
}