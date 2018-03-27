using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Nano.Config
{
    /// <summary>
    /// Config Manager.
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Builds the <see cref="IConfiguration"/>.
        /// </summary>
        /// <returns>The <see cref="IConfiguration"/>.</returns>
        public static IConfiguration BuildConfiguration(params string[] args)
        {
            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            return new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
