using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nano.App.Startup.Tasks;

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
    /// <param name="startupTaskContext">The <see cref="Startup.StartupTaskContext"/>.</param>
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