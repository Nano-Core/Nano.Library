using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Startup.Abstractions;

namespace Nano.App.Startup;

/// <summary>
/// Provides a base implementation for a startup task in the Nano application.
/// Derive from this class to implement custom startup logic that runs during application initialization.
/// </summary>
/// <param name="logger">The <see cref="ILogger{T}"/> instance used for logging within the startup task.</param>
public abstract class BaseStartupTask(ILogger<BaseStartupTask> logger)
    : IStartupTask
{
    /// <summary>
    /// Gets the <see cref="ILogger"/> instance used for logging within the startup task.
    /// </summary>
    protected ILogger<BaseStartupTask> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Called when the startup task begins execution.
    /// Implement this method with the task's startup logic.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the startup operation.</returns>
    public abstract Task OnStartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when the startup task stops execution.
    /// Override this method to implement cleanup logic if needed.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the stop operation.</returns>
    public virtual Task OnStopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}