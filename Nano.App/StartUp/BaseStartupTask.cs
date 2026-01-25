using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nano.App.StartUp;

/// <summary>
/// Base class for implementing hosted startup tasks.
/// Startup tasks automatically track their progress using <see cref="StartupTaskContext"/>.
/// </summary>
public abstract class BaseStartupTask : IHostedService
{
    private readonly StartupTaskContext startupTaskContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseStartupTask"/> class.
    /// </summary>
    /// <param name="startupTaskContext">The <see cref="StartupTaskContext"/> used to track task completion.</param>
    protected BaseStartupTask(StartupTaskContext startupTaskContext)
    {
        this.startupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
    }

    /// <summary>
    /// Starts the hosted task and increments the <see cref="StartupTaskContext"/> counter.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that completes when startup begins.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.startupTaskContext
            .Increment();

        return this.OnStartAsync(cancellationToken);
    }

    /// <summary>
    /// Stops the hosted task and decrements the <see cref="StartupTaskContext"/> counter when complete.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that completes when the task stops.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.OnStopAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            this.startupTaskContext
                .Decrement();
        }
    }

    /// <summary>
    /// Called when the startup task begins execution.
    /// Implement this method with the task's startup logic.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the startup operation.</returns>
    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Called when the startup task stops execution.
    /// Override this method to implement cleanup logic if needed.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the stop operation.</returns>
    protected virtual Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}