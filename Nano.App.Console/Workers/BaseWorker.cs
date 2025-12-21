using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nano.App.Console.Workers;

/// <summary>
/// Base Worker.
/// </summary>
public abstract class BaseWorker : IHostedService, IDisposable
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Application Lifetime.
    /// </summary>
    protected IHostApplicationLifetime ApplicationLifetime { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/>.</param>
    protected BaseWorker(ILogger logger, IHostApplicationLifetime applicationLifetime)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
    }

    /// <inheritdoc />
    public abstract Task StartAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        this.ApplicationLifetime
            .StopApplication();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}