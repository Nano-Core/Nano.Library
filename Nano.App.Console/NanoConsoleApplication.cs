using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Config;
using Nano.App.Console.Config;
using Nano.App.Console.Extensions;
using Nano.App.Consts;
using Nano.App.Extensions;
using Nano.Common.Config;
using System;
using System.IO;

namespace Nano.App.Console;

/// <summary>
/// 
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console</remarks>
public sealed class NanoConsoleApplication : BaseApplication<IHost, HostApplicationBuilder>, IApplication
{
    private NanoConsoleApplication(HostApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    public IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// Creates a <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="args">The command-line args, if any.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IApplication ConfigureApp(params string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;
        var config = ConfigManager.BuildConfiguration(environment, args);
        var applicationName = config[nameof(BaseAppOptions.Name)] ?? AppDefaults.DEFAULT_APP_NAME;

        var applicationOptions = new HostApplicationBuilderSettings
        {
            Args = args,
            ApplicationName = applicationName,
            EnvironmentName = environment,
            ContentRootPath = root
        };

        var builder = Host.CreateApplicationBuilder(applicationOptions);

        builder.Configuration
            .AddConfiguration(config);

        builder.Services
            .AddNanoApp<ConsoleOptions>(builder.Configuration, out var consoleOptions)
            .AddNanoWorkers()
            .AddNanoCultureInfo(consoleOptions);

        return new NanoConsoleApplication(builder);
    }

    /// <inheritdoc cref="IApplication" />
    public IApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        return this;
    }
}