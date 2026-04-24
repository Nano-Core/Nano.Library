using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nano.App.Abstractions;

/// <summary>
/// Defines the base contract for a Nano application.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Runs the application and blocks until shutdown.
    /// </summary>
    void Run();
}

/// <summary>
/// Defines the base contract for a Nano application.
/// Provides a fluent lifecycle for configuring services, building, and running the application.
/// </summary>
/// <remarks>
/// Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App">Nano Application</see>
/// </remarks>
public interface IApplication<out TApp> : IApplication
    where TApp : IApplication<TApp>
{
    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    /// <param name="configure">A delegate used to register services.</param>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
    TApp ConfigureServices(Action<IServiceCollection> configure);
}