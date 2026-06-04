using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.App.StartUp;

namespace Nano.App.Api.Mvc.HealthChecks;

internal sealed class StartupHealthCheck(StartupTaskContext taskContext) : IHealthCheck
{
    private readonly StartupTaskContext startupTaskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));

    /// <summary>
    /// Checks the health of the application based on the completion status of startup tasks.
    /// </summary>
    /// <param name="context">The <see cref="HealthCheckContext"/> provided by the health check system.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check.</param>
    /// <returns>
    ///     A <see cref="Task{HealthCheckResult}"/> representing the result of the health check.
    ///     Returns <see cref="HealthCheckResult.Healthy"/> if all startup tasks are completed; otherwise, <see cref="HealthCheckResult.Unhealthy"/>.
    /// </returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = this.startupTaskContext.IsDone
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();

        return Task.FromResult(result);
    }
}