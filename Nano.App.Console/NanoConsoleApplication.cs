using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Console.Config;
using Nano.App.Console.Extensions;
using Nano.App.Extensions;
using Nano.Common.Config;
using System;
using System.IO;
using System.Reflection;

namespace Nano.App.Console;

/// <inheritdoc />
public sealed class NanoConsoleApplication : BaseApplication<IHost, HostApplicationBuilder>
{
    private NanoConsoleApplication(HostApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Creates a <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="args">The command-line args, if any.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static NanoConsoleApplication ConfigureApp(params string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var config = ConfigManager.BuildConfiguration(args);
        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

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

    /// <inheritdoc />
    public override IApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        return this;
    }
}