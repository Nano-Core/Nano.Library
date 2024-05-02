using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App;
using Nano.App.Extensions;
using Nano.App.Interfaces;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Console.Extensions;
using Nano.Data.Extensions;
using Nano.Eventing.Extensions;
using Nano.Logging.Extensions;
using Nano.Security.Extensions;

namespace Nano.Console;

/// <inheritdoc />
public class ConsoleApplication : DefaultApplication
{
    /// <inheritdoc />
    public ConsoleApplication(IConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Creates a <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="args">The command-line args, if any.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder ConfigureApp(params string[] args)
    {
        return ConsoleApplication
            .ConfigureApp<ConsoleApplication>(args);
    }

    /// <summary>
    /// Creates a <see cref="IHostBuilder"/>.
    /// The application startup implementation is defined by the generic type parameter <typeparamref name="TApplication"/>.
    /// </summary>
    /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
    /// <param name="args">The command-line args, if any.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder ConfigureApp<TApplication>(params string[] args)
        where TApplication : class, IApplication
    {
        var root = Directory.GetCurrentDirectory();
        var config = ConfigManager.BuildConfiguration(args);
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        return new HostBuilder()
            .UseContentRoot(root)
            .UseEnvironment(environment)
            .UseConsoleLifetime()
            .ConfigureServices(x =>
            {
                x.AddSingleton<IApplication, TApplication>();

                x.AddApp(config);
                x.AddData(config);
                x.AddConfig(config);
                x.AddLogging(config);
                x.AddSecurity(config);
                x.AddEventing(config);
                x.AddConsole(config);
                x.AddApis(config);

                x.AddSingleton(x);
            });
    }
}