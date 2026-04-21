using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.Common.Config;
using System;
using System.IO;
using System.Reflection;

namespace Nano.App;

/// <summary>
/// Represents a base abstract application with a host and builder.
/// Provides common functionality for running the application.
/// </summary>
/// <typeparam name="TApp">The type of application, a type of <see cref="IApplication"/></typeparam>
/// <typeparam name="THost">The type of host (e.g., <see cref="IHost"/> or <see cref="WebApplication"/>).</typeparam>
/// <typeparam name="THostBuilder">The type of host builder (e.g., <see cref="IHostApplicationBuilder"/> or <see cref="WebApplicationBuilder"/>).</typeparam>
public abstract class BaseNanoApplication<TApp, THost, THostBuilder> : IApplication
    where TApp : IApplication<TApp>
    where THost : class, IHost
    where THostBuilder : IHostApplicationBuilder
{
    /// <summary>
    /// The application instance built from the builder.
    /// </summary>
    protected THost? application;

    /// <summary>
    /// The builder used to configure the application.
    /// </summary>
    protected THostBuilder applicationBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNanoApplication{TApp,THost,THostBuilder}"/> class.
    /// </summary>
    /// <param name="builder">The application builder used to configure services and middleware.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
    protected BaseNanoApplication(THostBuilder builder)
    {
        this.applicationBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <inheritdoc />
    public virtual void Run()
    {
        if (this.application == null)
        {
            throw new InvalidOperationException("No Application has been configured and Build.");
        }

        this.application
            .Run();
    }

    /// <summary>
    /// Creates and configures a <see cref="WebApplicationBuilder"/> with Nano defaults for web-based applications.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="WebApplicationBuilder"/> instance.</returns>
    protected static WebApplicationBuilder CreateWebBuilder(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var wwwroot = Path.Combine(root, "wwwroot");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;

        var entryAssembly = Assembly.GetEntryAssembly();
        var config = ConfigManager.BuildConfiguration(environment, entryAssembly, args);
        var applicationName = entryAssembly?.GetName().Name;

        var options = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = applicationName,
            EnvironmentName = environment,
            ContentRootPath = root,
            WebRootPath = wwwroot
        };

        var builder = WebApplication
            .CreateBuilder(options);

        builder.Configuration
            .AddConfiguration(config);
        // BUG
        //builder.Logging
        //    .ClearProviders();

        return builder;
    }

    /// <summary>
    /// Creates and configures a <see cref="HostApplicationBuilder"/> with Nano defaults for console-based applications.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="HostApplicationBuilder"/> instance.</returns>
    protected static HostApplicationBuilder CreateConsoleBuilder(string[] args)
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

        // BUG
        //builder.Logging
        //    .ClearProviders();

        return builder;
    }
}