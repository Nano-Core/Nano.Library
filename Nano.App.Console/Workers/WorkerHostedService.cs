using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Console.Workers.Abstractions;
using Nano.App.StartUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nano.App.Console.Workers;

/// <summary>
/// A hosted service that manages the lifecycle of multiple <see cref="IWorker"/> instances.
/// <para>
///     This service waits for all startup tasks tracked by <see cref="StartupTaskContext"/> to complete, then starts all registered workers concurrently.
///     Once all workers have completed their startup logic, it stops the workers concurrently and signals the host application to shut
///     down via <see cref="IHostApplicationLifetime.StopApplication"/>.
/// </para>
/// <para>
///     This class is intended to be registered as a singleton <see cref="IHostedService"/> in the host's dependency injection container.
/// </para>
/// </summary>
/// <param name="logger">The <see cref="ILogger"/>.</param>
/// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
/// <param name="startupTaskContext">The <see cref="startupTaskContext"/>.</param>
/// <param name="applicationLifetime">The <see cref="IHostApplicationBuilder"/>.</param>
public sealed class WorkerHostedService(ILogger<WorkerHostedService> logger, IServiceScopeFactory scopeFactory, StartupTaskContext startupTaskContext, IHostApplicationLifetime applicationLifetime)
    : IHostedService
{
    private readonly ILogger<WorkerHostedService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceScopeFactory scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly StartupTaskContext startupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));
    private readonly IHostApplicationLifetime applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));

    /// <summary>
    /// Starts all registered workers after all startup tasks have completed. This method waits for the <see cref="StartupTaskContext"/> to complete,
    /// then invokes <see cref="IWorker.OnStartAsync(CancellationToken)"/> on each worker concurrently, and finally calls <see cref="StopAsync(CancellationToken)"/> to
    /// signal application shutdown.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for startup tasks and executing worker start operations. Cancelling this token will abort the operation.</param>
    /// <returns>A <see cref="Task"/> that completes when all workers have started and the stop sequence has been initiated.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.startupTaskContext.Completion
            .WaitAsync(cancellationToken);

        using var scope = this.scopeFactory
            .CreateScope();

        var workers = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IWorker>>();

        var startTasks = workers
            .Select(x =>
            {
                try
                {
                    return x.OnStartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    this.logger
                        .LogError(ex, ex.Message);
                }

                return Task.CompletedTask;
            });

        await Task.WhenAll(startTasks)
            .ConfigureAwait(false);

        await this.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Stops all registered workers by invoking <see cref="IWorker.OnStopAsync(CancellationToken)"/> concurrently, and signals the host
    /// to stop the application via <see cref="IHostApplicationLifetime.StopApplication"/>.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while stopping each worker. Cancelling this token will abort the stop operation.</param>
    /// <returns>A <see cref="Task"/> that completes when all workers have stopped and the application shutdown has been triggered.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = this.scopeFactory
            .CreateScope();

        var workers = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IWorker>>();

        var stopTasks = workers
            .Select(x =>
            {
                try
                {
                    return x.OnStopAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    this.logger
                        .LogError(ex, ex.Message);
                }

                return Task.CompletedTask;
            });

        await Task.WhenAll(stopTasks)
            .ConfigureAwait(false);

        this.applicationLifetime
            .StopApplication();
    }
}