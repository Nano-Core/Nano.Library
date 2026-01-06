using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Nano.App.StartUp;

/// <inheritdoc />
public abstract class BaseStartupTask : IHostedService
{
    private readonly StartupTaskContext startupTaskContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="startupTaskContext">The <see cref="StartupTaskContext"/>.</param>
    protected BaseStartupTask(StartupTaskContext startupTaskContext)
    {
        this.startupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.startupTaskContext
            .Increment();

        return this.OnStartAsync(cancellationToken);
    }

    /// <inheritdoc />
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
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task OnStartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}