using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nano.App.StartUp;

namespace Nano.App.Console.Workers;

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
    /// Gets the <see cref="StartupTaskContext"/> to monitor the application startup.
    /// </summary>
    protected StartupTaskContext StartupTaskContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseWorker"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/> for application lifecycle management.</param>
    /// <param name="startupTaskContext">The <see cref="StartupTaskContext"/> for the application startup.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> or <paramref name="applicationLifetime"/> is null.</exception>
    protected BaseWorker(ILogger logger, IHostApplicationLifetime applicationLifetime, StartupTaskContext startupTaskContext)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        this.StartupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
    }

    /// <summary>
    /// Starts the worker asynchronously. Must be implemented by derived classes.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.StartupTaskContext.Completion
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        await this.OnStartAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Called when the worker starts execution.
    /// Implement this method to perform the worker's startup logic.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to observe cancellation requests during startup.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous startup operation.</returns>
    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

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