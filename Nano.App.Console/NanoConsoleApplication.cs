using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Console.Config;
using Nano.App.Console.Extensions;
using Nano.App.Extensions;
using System;

namespace Nano.App.Console;

/// <summary>
/// Represents a Nano console application designed to run as a background service or cron job.
/// Provides an entry point for configuring services and running console-based Nano applications.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Console">Nano Console Application</see></remarks>
public class NanoConsoleApplication : BaseNanoApplication<IHost, HostApplicationBuilder>
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
    public new static IApplication ConfigureApp(params string[] args)
    {
        var builder = BaseNanoApplication.CreateConsoleBuilder(args);

        builder.Services
            .AddNanoApp<ConsoleOptions>(builder.Configuration, out var options);

        builder.Services
            .ConfigureNanoConsoleServices(options);

        return new NanoConsoleApplication(builder);
    }

    /// <summary>
    /// Builds the application and prepares it for execution.
    /// </summary>
    /// <param name="applicationBuilderAction">The <see cref="IApplicationBuilder"/>.</param>
    /// <remarks>
    ///     The <paramref name="applicationBuilderAction"/> has NO effect in <see cref="NanoConsoleApplication"/>.
    /// </remarks>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
    public override IApplication Build(Action<IApplicationBuilder>? applicationBuilderAction = null)
    {
        this.application = this.applicationBuilder
            .Build();

        return this;
    }
}