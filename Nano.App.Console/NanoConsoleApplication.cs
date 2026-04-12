using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Console.Config;
using Nano.App.Console.Extensions;
using Nano.App.Extensions;
using Nano.Eventing.Abstractions.Extensions;
using System;
using Nano.Data.Abstractions.Eventing.Extensions;
using Nano.Data.Abstractions.Extensions;

namespace Nano.App.Console;

/// <summary>
/// Represents a Nano console application designed to run as a background service or cron job.
/// Provides an entry point for configuring services and running console-based Nano applications.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console">Nano Console Application</see></remarks>
public class NanoConsoleApplication : BaseNanoApplication<IConsoleApplication, IHost, HostApplicationBuilder>, IConsoleApplication
{
    /// <summary>
    /// Initializes an instance of <see cref="NanoConsoleApplication"/>.
    /// </summary>
    /// <param name="builder">The <see cref="HostApplicationBuilder"/>.</param>
    protected NanoConsoleApplication(HostApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Creates and configures the console application with default Nano services and options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    public static IConsoleApplication ConfigureApp(params string[] args)
    {
        var builder = CreateConsoleBuilder(args);

        builder.Services
            .AddNanoApp<ConsoleOptions>(builder.Configuration, out var options);

        builder.Services
            .ConfigureNanoConsoleServices(options);

        return new NanoConsoleApplication(builder);
    }

    /// <inheritdoc />
    public virtual IConsoleApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(this.applicationBuilder.Services);

        return this;
    }

    /// <inheritdoc />
    public virtual IConsoleApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        using var serviceScope = this.application.Services
            .CreateScope();

        serviceScope
            .UseEventHandlers()
            .UseEntityEventing()
            .UseNanoDbMigrations();

        return this;
    }
}