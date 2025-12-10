using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nano.App.StartUp.Tasks;

// BUG: This is meant for consumers to add startup tasks. We need to find a way to automatically increment  / decrement Context, so the consumer doesn't have to remember it

/// <inheritdoc />
public abstract class BaseStartupTask : IHostedService
{
    /// <summary>
    /// Context.
    /// </summary>
    protected internal virtual StartupTaskContext Context { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="startupTaskContext">The <see cref="StartupTaskContext"/>.</param>
    protected BaseStartupTask(StartupTaskContext startupTaskContext)
    {
        this.Context = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
    }

    /// <inheritdoc />
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}