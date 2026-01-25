using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nano.App.Console.Workers;

// BUG: Should block this worker until startup tasks are complete

/// <summary>
/// Provides a base implementation for workers implementing <see cref="IHostedService"/>.
/// Supports logging and application lifetime management, and implements <see cref="IDisposable"/>.
/// </summary>
public abstract class BaseWorker : IHostedService, IDisposable
{
    /// <summary>
    /// Gets the <see cref="ILogger"/> used for logging within the worker.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="IHostApplicationLifetime"/> to manage the application lifecycle.
    /// </summary>
    protected IHostApplicationLifetime ApplicationLifetime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseWorker"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/> for application lifecycle management.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> or <paramref name="applicationLifetime"/> is null.</exception>
    protected BaseWorker(ILogger logger, IHostApplicationLifetime applicationLifetime)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
    }

    /// <summary>
    /// Starts the worker asynchronously. Must be implemented by derived classes.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the worker asynchronously and triggers application shutdown.
    /// Can be overridden in derived classes to perform additional cleanup or shutdown logic.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the stop operation.</param>
    /// <returns>A <see cref="Task"/> that represents the completion of the stop operation.</returns>
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        this.ApplicationLifetime
            .StopApplication();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
    /// </summary>
    public virtual void Dispose()
    {
    }
}