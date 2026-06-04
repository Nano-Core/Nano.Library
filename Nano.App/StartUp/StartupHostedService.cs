using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nano.App.StartUp.Abstractions;

namespace Nano.App.StartUp;

/// <summary>
/// A hosted service that runs all registered <see cref="IStartupTask"/> instances during application startup
/// and ensures proper cleanup when stopping. It also tracks task completion using <see cref="StartupTaskContext"/>.
/// </summary>
/// <param name="logger">The <see cref="ILogger{T}"/> used for logging errors and status.</param>
/// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/> used to create a service scope for resolving tasks.</param>
/// <param name="startupTaskContext">The <see cref="StartupTaskContext"/> used to track running startup tasks.</param>
public sealed class StartupHostedService(ILogger<StartupHostedService> logger, IServiceScopeFactory scopeFactory, StartupTaskContext startupTaskContext)
    : IHostedService
{
    private readonly ILogger<StartupHostedService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceScopeFactory scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    private readonly StartupTaskContext startupTaskContext = startupTaskContext ?? throw new ArgumentNullException(nameof(startupTaskContext));

    /// <summary>
    /// Executes all registered startup tasks asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while performing the startup tasks.</param>
    /// <returns>A <see cref="Task"/> representing the completion of all startup tasks.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this.scopeFactory
            .CreateScope();

        var workers = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IStartupTask>>();

        var startTasks = workers
            .Select(x =>
            {
                try
                {
                    this.startupTaskContext
                        .Increment();

                    return x.OnStartAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger
                        .LogError(ex, ex.Message);

                    throw;
                }
            });

        await Task.WhenAll(startTasks)
            .ConfigureAwait(false);

        await this.StopAsync(cancellationToken);
    }

    /// <summary>
    /// Executes the stop logic for all registered startup tasks asynchronously
    /// and decrements the <see cref="StartupTaskContext"/> count.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while performing the stop tasks.</param>
    /// <returns>A <see cref="Task"/> representing the completion of all stop operations.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = this.scopeFactory
            .CreateScope();

        var workers = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IStartupTask>>();

        var stopTasks = workers
            .Select(async x =>
            {
                try
                {
                    await x.OnStopAsync(cancellationToken);

                    this.startupTaskContext
                        .Decrement();
                }
                catch (Exception ex)
                {
                    this.logger
                        .LogError(ex, ex.Message);

                    throw;
                }
            });

        await Task.WhenAll(stopTasks)
            .ConfigureAwait(false);
    }
}