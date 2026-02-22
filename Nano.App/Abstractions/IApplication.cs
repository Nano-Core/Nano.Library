using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nano.App.Abstractions;

/// <summary>
/// Defines the contract for a Nano application.
/// Provides a fluent lifecycle for configuring services, building, and running the application.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App">Nano Application</see>
/// </remarks>
public interface IApplication
{
    /// <summary>
    /// Creates and configures a new application instance.
    /// Acts as the entry point for application setup.
    /// </summary>
    /// <param name="args">Optional command-line arguments.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    static abstract IApplication ConfigureApp(params string[] args);

    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    /// <param name="configure">A delegate used to register services.</param>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
    IApplication ConfigureServices(Action<IServiceCollection> configure);

    /// <summary>
    /// Builds the application and finalizes configuration.
    /// Must be called before <see cref="Run"/>.
    /// </summary>
    /// <param name="applicationBuilderAction">The <see cref="IApplicationBuilder"/>.</param>
    IApplication Build(Action<IApplicationBuilder>? applicationBuilderAction = null);

    /// <summary>
    /// Runs the application and blocks until shutdown.
    /// </summary>
    void Run();
}