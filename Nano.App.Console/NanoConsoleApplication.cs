using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

/// <summary>
/// Represents a Nano console application designed to run as a background service or cron job.
/// Provides an entry point for configuring services and running console-based Nano applications.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console">Nano Console Application</see></remarks>
public sealed class NanoConsoleApplication : BaseNanoApplication<IHost, HostApplicationBuilder>, IApplication
{
    private NanoConsoleApplication(HostApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Allows consumers to register services for the application.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="IServiceCollection"/>.</param>
    /// <returns>The current <see cref="IApplication"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configure"/> is null.</exception>
    public IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// Creates and configures the console application with default Nano services and options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    public static IApplication ConfigureApp(params string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;
        var entryAssembly = Assembly.GetEntryAssembly();
        var config = ConfigManager.BuildConfiguration(environment, entryAssembly, args);
        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name;

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
            .AddNanoCultureInfo(consoleOptions.Localization);

        return new NanoConsoleApplication(builder);
    }

    /// <summary>
    /// Builds the application and prepares it for execution.
    /// </summary>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
    public IApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        return this;
    }
}