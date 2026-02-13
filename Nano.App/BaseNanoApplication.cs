using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Builder;

namespace Nano.App;

/// <summary>
/// Represents a base abstract application with a host and builder.
/// Provides common functionality for running the application.
/// </summary>
/// <typeparam name="THost">The type of host (e.g., <see cref="IHost"/> or <see cref="WebApplication"/>).</typeparam>
/// <typeparam name="THostBuilder">The type of host builder (e.g., <see cref="IHostApplicationBuilder"/> or <see cref="WebApplicationBuilder"/>).</typeparam>
public abstract class BaseNanoApplication<THost, THostBuilder>
    where THost : class, IHost
    where THostBuilder : IHostApplicationBuilder
{
    /// <summary>
    /// The application instance built from the builder.
    /// </summary>
    protected THost application = null!;

    /// <summary>
    /// The builder used to configure the application.
    /// </summary>
    protected THostBuilder applicationBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNanoApplication{THost,THostBuilder}"/> class.
    /// </summary>
    /// <param name="builder">The application builder used to configure services and middleware.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> is null.</exception>
    protected BaseNanoApplication(THostBuilder builder)
    {
        this.applicationBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Runs the application. The <see cref="application"/> must be built before calling this method.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the application has not been configured and built.</exception>
    public virtual void Run()
    {
        if (this.application == null)
        {
            throw new InvalidOperationException("No Application has been configured and Build.");
        }

        this.application
            .Run();
    }
}