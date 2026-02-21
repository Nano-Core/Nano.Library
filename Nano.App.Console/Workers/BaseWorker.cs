using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Console.Workers.Abstractions;

namespace Nano.App.Console.Workers;

/// <summary>
/// Represents the base implementation for a worker that can be started and stopped by the application host.
/// <para>
/// Derived classes must implement <see cref="OnStartAsync(CancellationToken)"/> to define their startup behavior.
/// <see cref="OnStopAsync(CancellationToken)"/> can optionally be overridden to define cleanup or shutdown logic.
/// </para>
/// </summary>
public abstract class BaseWorker(ILogger logger) 
    : IWorker
{
    /// <summary>
    /// Gets the <see cref="ILogger"/> instance used for logging within the worker.
    /// </summary>
    protected ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Performs the asynchronous startup logic for the worker. Derived classes must implement this method.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the startup operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous startup operation.</returns>
    public abstract Task OnStartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs the asynchronous shutdown logic for the worker. Can be overridden in derived classes to perform cleanup or stopping logic.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the stop operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous stop operation. By default, returns a completed task.</returns>
    public virtual Task OnStopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}